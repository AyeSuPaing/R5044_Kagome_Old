<%@ WebHandler Language="C#" Class="ProductRegistOrUpdateProcess" %>
/*
=========================================================================================================
  Module      : Product Regist Or Update Process (ProductRegistOrUpdateProcess.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using w2.App.Common.Input;
using w2.App.Common.Preview;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.ShopOperator;

/// <summary>
/// Product regist or update process
/// </summary>
public class ProductRegistOrUpdateProcess : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    /// <summary>File type</summary>
    public enum FileType
    {
        Unknown,
        Jpeg,
        Bmp,
        Gif,
        Png
    }

    /// <summary>Known file headers</summary>
    private static readonly Dictionary<FileType, byte[]> KNOWN_FILE_HEADERS = new Dictionary<FileType, byte[]>
    {
        { FileType.Jpeg, new byte[] { 0xFF, 0xD8 } }, // JPEG
		{ FileType.Bmp, new byte[] { 0x42, 0x4D } }, // BMP
		{ FileType.Gif, new byte[] { 0x47, 0x49, 0x46 } }, // GIF
		{ FileType.Png, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } }, // PNG
	};

    /// <summary>
    /// Process request
    /// </summary>
    /// <param name="context">Context</param>
    public void ProcessRequest(HttpContext context)
    {
        string response;
        switch (this.ActionKbn)
        {
            case Constants.PRODUCT_PROCESS_ACTION_KBN_UPDATE_PREVIEW:
            case Constants.PRODUCT_PROCESS_ACTION_KBN_REGIST_PREVIEW:
                response = RegistProductReviewProcess(context);
                break;

            case Constants.ACTION_STATUS_INSERT:
            case Constants.ACTION_STATUS_COPY_INSERT:
            case Constants.ACTION_STATUS_UPDATE:
                response = MoveToProductConfirmProcess(context);
                break;

            case Constants.PRODUCT_PROCESS_ACTION_KBN_IMPORT:
                response = ImportFileProcess(context);
                break;

            case Constants.PRODUCT_PROCESS_ACTION_KBN_UPLOAD_IMAGE_INSERT:
                response = UploadImagesForInsert(context);
                break;

            case Constants.PRODUCT_PROCESS_ACTION_KBN_UPLOAD_IMAGE_UPDATE_OR_COPY_INSERT:
                response = UploadImagesForUpdateOrCopyInsert(context);
                break;

            default:
                response = string.Empty;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.Write(response);
    }

    /// <summary>
    /// Import file process
    /// </summary>
    /// <param name="context">Context</param>
    /// <returns>A result as json</returns>
    public string ImportFileProcess(HttpContext context)
    {
        var response = new ProductMasterProcessResponse();

        try
        {
            // Check file exist & have content
            if ((context.Request.Files.Count == 0)
                || (context.Request.Files[0].ContentLength == 0)
                || (context.Request.Files[0].InputStream.Length == 0))
            {
                response.ErrorMessage =
                    WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_FILE_UNFIND);
                return ConvertObjectToJsonString(response);
            }

            // Check file extension
            var file = context.Request.Files[0];
            if (file.FileName.EndsWith(".csv") == false)
            {
                response.ErrorMessage =
                    WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_FILE_NOT_CSV);
                return ConvertObjectToJsonString(response);
            }

            var shopDirectory = Path.Combine(Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR, this.LoggedInShopOperator.ShopId);
            var importMasterType = StringUtility.ToEmpty(context.Request.Form[Constants.PARAM_PRODUCT_IMPORT_MASTER_TYPE]);
            var uploadDirectory = Path.Combine(shopDirectory, importMasterType, Constants.DIRNAME_MASTERIMPORT_UPLOAD);

            // Ensure upload directory exist
            EnsureUploadImageTempPathExist(uploadDirectory);

            // Check file upload has exist
            var uploadFilePath = Path.Combine(uploadDirectory, Path.GetFileName(file.FileName));
            if (File.Exists(uploadFilePath))
            {
                response.ErrorMessage =
                    WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_ALREADY_EXISTS);
                return ConvertObjectToJsonString(response);
            }

            // Save upload file
            file.SaveAs(uploadFilePath);

            // Check active directory exist
            var activeDirectory = Path.Combine(shopDirectory, importMasterType, Constants.DIRNAME_MASTERIMPORT_ACTIVE);
            var activeFilePath = Path.Combine(activeDirectory, file.FileName);
            EnsureUploadImageTempPathExist(activeDirectory);

            if (File.Exists(uploadFilePath))
            {
                // ファイル移動（プロセス生成に時間がかかることがあるため、移動後のファイルをバッチへ渡す。）
                File.Move(uploadFilePath, activeFilePath);

                // プロセス実行（移動後ファイルのフルパスを引数として渡す。）
                // スペースが含まれても処理されるように「"」でくくる
                Process.Start(
                    Constants.PHYSICALDIRPATH_MASTERUPLOAD_EXE,
                    string.Format("\"{0}\"", activeFilePath));
            }

            response.IsSuccess = true;
            return ConvertObjectToJsonString(response);
        }
        catch (Exception ex)
        {
            FileLogger.WriteError(ex);

            response.ErrorMessage =
                WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_UPLOAD_ERROR);
            return ConvertObjectToJsonString(response);
        }
    }

    /// <summary>
    /// Get input from request
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="isPreview">Is preview</param>
    /// <returns>Product input</returns>
    private ProductInput GetInputFromRequest(HttpContext context, bool isPreview = false)
    {
        var request = context.Request.Form[Constants.PARAM_PRODUCT_INPUT];
        var input = BasePageHelper.DeserializeJsonObject<ProductInput>(request);
        var refProductId = context.Request.Form[Constants.PARAM_REFERENCE_PRODUCT_ID];
        input.ShopId = this.LoggedInShopOperator.ShopId;
        input.LastChanged = this.LoggedInShopOperator.Name;

        if ((this.ActionKbn == Constants.ACTION_STATUS_UPDATE)
            || (this.ActionKbn == Constants.PRODUCT_PROCESS_ACTION_KBN_UPDATE_PREVIEW))
        {
            input.ProductIdOld = input.ProductId;
        }

        if (Constants.PRODUCT_BRAND_ENABLED)
        {
            input.BrandId1ForCheck = input.BrandId1;
        }

        // Product price
        if (isPreview)
        {
            input.ProductPrices = new ProductPriceInput[0];
        }
        else
        {
            foreach (var productPrice in input.ProductPrices)
            {
                productPrice.LastChanged = input.LastChanged;
                productPrice.IsSetProductPrice = (string.IsNullOrEmpty(productPrice.MemberRankPrice) == false);
            }
        }

        // バリエーションを使用する場合
        if (input.HasProductVariation)
        {
            foreach (var variation in input.ProductVariations)
            {
                variation.LastChanged = input.LastChanged;

                if (isPreview)
                {
                    variation.ProductPrices = new ProductPriceInput[0];
                }
                else
                {
                    // Create product price for variation
                    foreach (var productPrice in variation.ProductPrices)
                    {
                        productPrice.LastChanged = input.LastChanged;
                        productPrice.IsSetProductPrice = (string.IsNullOrEmpty(productPrice.MemberRankPrice) == false);
                    }
                }

                variation.MallVariationId1 =
                    (Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION && string.IsNullOrEmpty(variation.MallVariationId1))
                        ? variation.VariationId
                        : variation.MallVariationId1;
            }
        }

        // Product tag
        if (input.HasProductTag)
        {
            input.ProductTag.LastChanged = input.LastChanged;
            input.ProductTag.ProductId = input.ProductId;
        }

        // Product extend
        if (input.HasProductExtend)
        {
            input.ProductExtend.LastChanged = input.LastChanged;
            input.ProductExtend.ProductId = input.ProductId;
            input.ProductExtend.ShopId = input.ShopId;
        }

        // Mall exhibits config
        if (Constants.MALLCOOPERATION_OPTION_ENABLED)
        {
            input.MallExhibitsConfig.LastChanged = input.LastChanged;
            input.MallExhibitsConfig.ShopId = input.ShopId;
            input.MallExhibitsConfig.ProductId = input.ProductId;
        }

        // Product fixed purchase discount setting
        if (input.HasProductFixedPurchaseDiscountSettings)
        {
            foreach (var productFixedPurchaseDiscount in input.ProductFixedPurchaseDiscountSettings)
            {
                productFixedPurchaseDiscount.ShopId = input.ShopId;
                productFixedPurchaseDiscount.ProductId = input.ProductId;
            }

            input.ProductFixedPurchaseDiscountSettings =
                GetSortedProductFixedPurchaseDiscount(input.ProductFixedPurchaseDiscountSettings);
        }

        return input;
    }

    /// <summary>
    /// Get sorted product fixedpurchase discount
    /// </summary>
    /// <param name="productFixedPurchaseDiscount">Product fixedpurchase discount</param>
    /// <returns>A array sorted of product fixedpurchase discount</returns>
    private ProductFixedPurchaseDiscountSettingHeader[] GetSortedProductFixedPurchaseDiscount(
        ProductFixedPurchaseDiscountSettingHeader[] productFixedPurchaseDiscount)
    {
        int parsedProductCount;
        foreach (var row in productFixedPurchaseDiscount)
        {
            // 数値以外が含まれる場合、個数でのソートを行わない
            var invalidNumberCount = row.ProductCountDiscounts
                .Count(col => (int.TryParse(col.ProductCount, out parsedProductCount) == false));
            if (invalidNumberCount > 0) continue;

            row.ProductCountDiscounts = row.ProductCountDiscounts
                .OrderBy(col => int.Parse(col.ProductCount))
                .ToList();
        }

        // 数値以外が含まれる場合、回数でのソートを行わない
        int parsedOrderCount;
        var productFixedPurchaseDiscountSorted =
            (productFixedPurchaseDiscount.Count(row => (int.TryParse(row.OrderCount, out parsedOrderCount) == false)) > 0)
                ? productFixedPurchaseDiscount
                : productFixedPurchaseDiscount.OrderBy(row => int.Parse(row.OrderCount)).ToArray();

        return productFixedPurchaseDiscountSorted;
    }

    /// <summary>
    /// Regist product review process
    /// </summary>
    /// <param name="context">Context</param>
    /// <returns>A result as json</returns>
    private string RegistProductReviewProcess(HttpContext context)
    {
        var input = GetInputFromRequest(context, true);
        var response = new ProductMasterProcessResponse();

        // Validate process
        var errorMessages = ValidateInput(input);
        if (string.IsNullOrEmpty(errorMessages) == false)
        {
            response.ErrorMessage = errorMessages;
            return ConvertObjectToJsonString(response);
        }

        // Insert process
        var newInput = new ProductInput((Hashtable)input.DataSource.Clone());
        var model = newInput.CreateModel();
        DataView tmpProduct = null;
        DataTable product = null;
        var productPage = new ProductPage();

        using (var accessor = new SqlAccessor())
        {
            // トランザクション開始
            accessor.OpenConnection();
            accessor.BeginTransaction();

            try
            {
                // 商品情報登録・更新
                productPage.InsertUpdateProduct(
                    (this.ActionKbn == Constants.PRODUCT_PROCESS_ACTION_KBN_UPDATE_PREVIEW)
                        ? Constants.ACTION_STATUS_UPDATE
                        : Constants.ACTION_STATUS_INSERT,
                    model,
                    accessor);

                // 商品情報取得
                tmpProduct = ProductPreview.GetProductDetailPreviewData(
                    model.ShopId,
                    model.ProductId,
                    accessor);

                // 該当なしの場合、エラーページへ
                if (tmpProduct.Count == 0)
                {
                    response.ErrorMessage =
                        WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
                    return ConvertObjectToJsonString(response);
                }

                // データ取得
                product = tmpProduct.Table;
            }
            catch (Exception ex)
            {
                FileLogger.WriteError(ex);
                response.ErrorMessage =
                    WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR);
                return ConvertObjectToJsonString(response);
            }
        }

        // 商品詳細プレビュー情報登録
        ProductPreview.InsertProductDetailPreview(
            model.ShopId,
            model.ProductId,
            product);
        var targetSite = context.Request.Form[Constants.PARAM_PRODUCT_PREVIEW_SITE];
        input.GuidString = this.GuidString;
        SessionManager.Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO] = input;

        if (Directory.Exists(this.UploadImageTempPath) && (Constants.PRODUCT_IMAGE_HEAD_ENABLED == false))
        {
            response.GuidString = string.Format("{0:yyyyMMdd}_{1}", DateTime.Now, Guid.NewGuid());
            CreateNewImageTempPath(response.GuidString);
        }
        else
        {
            response.GuidString = this.GuidString;
        }

        response.IsSuccess = true;
        response.ReviewUrl = productPage.CreateUrlForProductDetailPreview(
            targetSite,
            model.ProductId,
            Constants.PRODUCT_BRAND_ENABLED ? model.BrandId1 : null,
            response.GuidString);

        return ConvertObjectToJsonString(response);
    }

    /// <summary>
    /// Upload images for insert
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>Responce message</returns>
    private string UploadImagesForInsert(HttpContext context)
    {
        this.UploadImageInput = GetUploadImageInput(context);
        this.UploadImageInput.Guid = this.GuidString;
        SessionManager.Session[Constants.SESSIONPARAM_KEY_UPLODED_MASTER] = this.UploadImageInput;
        return string.Empty;
    }

    /// <summary>
    /// Upload images for update or copy insert
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>Responce message</returns>
    private string UploadImagesForUpdateOrCopyInsert(HttpContext context)
    {
        this.UploadImageInput = GetUploadImageInput(context);
        this.UploadImageInput.Guid = this.GuidString;
        var hasError = false;

        if (this.IsBackFromConfirm == false)
        {
            // Get reference product
            var refProductId = context.Request.Form[Constants.PARAM_REFERENCE_PRODUCT_ID];
            var refProduct = DomainFacade.Instance.ProductService.Get(this.LoggedInShopOperator.ShopId, refProductId);
            // Handle product main image
            var mainImage = this.UploadImageInput.MainImage;
            if (string.IsNullOrEmpty(mainImage.FileName)
                && (mainImage.DelFlg == false)
                && (string.IsNullOrEmpty(refProduct.ImageHead) == false)
                && (string.IsNullOrEmpty(this.UploadImageInput.ImageHead) == false))
            {
                try
                {
                    var searchFileNamePattern = string.Empty;
                    if (mainImage.SourceIndex == 0)
                    {
                        searchFileNamePattern = string.Format(@"{0}{1}", refProduct.ImageHead, Constants.PRODUCTIMAGE_FOOTER);

                        CopyImagesToTempPath(
                            this.UploadImageMainPath,
                            searchFileNamePattern,
                            refProduct.ImageHead,
                            this.UploadImageInput.ImageHead);
                    }
                    else
                    {
                        searchFileNamePattern = CreateSubImageSearchFileNamePattern(mainImage.SourceIndex, refProduct.ImageHead);

                        CopyImagesToTempPath(
                            this.UploadImageSubPath,
                            searchFileNamePattern,
                            searchFileNamePattern.Replace(Constants.PRODUCTIMAGE_FOOTER, string.Empty),
                            this.UploadImageInput.ImageHead);
                    }

                    this.UploadImageInput.MainImage.FileName =
                        this.UploadImageInput.ImageHead + Constants.PRODUCTIMAGE_FOOTER_LL;
                }
                catch (Exception ex)
                {
                    FileLogger.WriteError(ex);
                    hasError = true;
                }
            }

            // Handle product variation image
            foreach (var image in this.UploadImageInput.VariationImages)
            {
                if (string.IsNullOrEmpty(image.FileName)
                    && (image.DelFlg == false)
                    && (string.IsNullOrEmpty(refProduct.ImageHead) == false)
                    && (string.IsNullOrEmpty(image.ImageHead) == false))
                {
                    try
                    {
                        // Find reference file
                        var refVariation = DomainFacade.Instance.ProductService.GetProductVariation(
                            this.LoggedInShopOperator.ShopId,
                            refProductId,
                            image.RefVariationId,
                            string.Empty);

                        var searchFileNamePattern = string.Format(@"{0}{1}", refVariation.VariationImageHead, Constants.PRODUCTIMAGE_FOOTER);

                        CopyImagesToTempPath(
                            this.UploadImageMainPath,
                            searchFileNamePattern,
                            refVariation.VariationImageHead,
                            image.ImageHead);

                        image.FileName = image.ImageHead + Constants.PRODUCTIMAGE_FOOTER_LL;
                    }
                    catch (Exception ex)
                    {
                        FileLogger.WriteError(ex);
                        hasError = true;
                    }
                }
            }

            // Handle product sub images
            foreach (var image in this.UploadImageInput.SubImages)
            {
                if (string.IsNullOrEmpty(image.FileName)
                    && (image.DelFlg == false)
                    && (string.IsNullOrEmpty(refProduct.ImageHead) == false)
                    && (string.IsNullOrEmpty(this.UploadImageInput.ImageHead) == false))
                {
                    try
                    {
                        var searchFileNamePattern = string.Empty;
                        var imageFileName = CreateSubImageFileName(image.ImageNo, this.UploadImageInput.ImageHead);
                        if (image.SourceIndex == 0)
                        {
                            searchFileNamePattern = string.Format(@"{0}{1}", refProduct.ImageHead, Constants.PRODUCTIMAGE_FOOTER);

                            CopyImagesToTempPath(
                                this.UploadImageMainPath,
                                searchFileNamePattern,
                                refProduct.ImageHead,
                                imageFileName.Replace(Constants.PRODUCTIMAGE_FOOTER_LL, string.Empty));
                        }
                        else
                        {
                            searchFileNamePattern = CreateSubImageSearchFileNamePattern(image.SourceIndex, refProduct.ImageHead);

                            CopyImagesToTempPath(
                                this.UploadImageSubPath,
                                searchFileNamePattern,
                                searchFileNamePattern.Replace(Constants.PRODUCTIMAGE_FOOTER, string.Empty),
                                imageFileName.Replace(Constants.PRODUCTIMAGE_FOOTER_LL, string.Empty));
                        }

                        image.FileName = imageFileName;
                    }
                    catch (Exception ex)
                    {
                        FileLogger.WriteError(ex);
                        hasError = true;
                    }
                }
            }

            // When image has resize, execute resize action
            if (this.UploadImageInput.AutoResize)
            {
                ResizeImage(this.UploadImageInput);
            }
            else
            {
                DeleteResizeImage();
            }
        }

        if (hasError)
        {
            context.Session[Constants.SESSION_KEY_ERROR_MSG] =
                WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_REGISTER_UPLOAD_IMAGE_FAILED);
        }

        SessionManager.Session[Constants.SESSIONPARAM_KEY_UPLODED_MASTER] = this.UploadImageInput;
        return string.Empty;
    }

    /// <summary>
    /// Copy images to temporary path
    /// </summary>
    /// <param name="path">An image path</param>
    /// <param name="searchFileNamePattern">Search file name pattern</param>
    /// <param name="oldName">Old name</param>
    /// <param name="newName">New name</param>
    private void CopyImagesToTempPath(
        string path,
        string searchFileNamePattern,
        string oldName,
        string newName)
    {
        var directoryInfo = new DirectoryInfo(path);
        var images = directoryInfo.GetFiles()
            .Where(file => Regex.Match(file.Name, searchFileNamePattern).Success)
            .ToList();

        foreach (var image in images)
        {
            var imageName = image.Name.Replace(oldName, newName);
            File.Copy(
                image.FullName,
                Path.Combine(this.UploadImageTempPath, imageName));
        }
    }

    /// <summary>
    /// Get upload image input
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>An upload image input</returns>
    private UploadImageInput GetUploadImageInput(HttpContext context)
    {
        EnsureUploadImageTempPathExist(this.UploadImageTempPath);

        var input = ConvertJsonStringToObject<UploadImageInput>(context.Request.Form[Constants.PARAM_PRODUCT_UPLOAD_IMAGE_INPUT]);
        if (Constants.PRODUCT_IMAGE_HEAD_ENABLED) return input;

        UpdateImageHead(input);

        // Because when user press back button from ProductConfirm the user can change images.
        // If user swap any image positions, the name of image need to change to correct the swap action.
        // Ex: If you swap image_sub1.jpg & image_sub2.jpg then you need to rename image_sub1.jpg to image_sub2.jpg & vice versa.
        RenameImages(input);

        if (context.Request.Files.Count != 0)
        {
            // Save product main image
            if (input.MainImage.IsNewFile)
            {
                var destFileName = input.ImageHead + Constants.PRODUCTIMAGE_FOOTER_LL;
                input.MainImage.FileName = SaveFile(context, destFileName, 0);
                input.MainImage.DelFlg = false;
            }

            // Save product sub image sources
            foreach (var image in input.SubImages.Where(item => item.IsNewFile))
            {
                var destFileName = CreateSubImageFileName(image.ImageNo, input.ImageHead);
                image.FileName = SaveFile(context, destFileName, image.FileIndex);
                image.DelFlg = false;
            }

            // Save product variation image sources
            foreach (var image in input.VariationImages.Where(item => item.IsNewFile))
            {
                var variationDesName = image.ImageHead + Constants.PRODUCTIMAGE_FOOTER_LL;
                image.FileName = SaveFile(context, variationDesName, image.FileIndex);
                image.DelFlg = false;
            }
        }

        DeleteUnusedImages(input);

        // When image has resize, execute resize action
        if (input.AutoResize)
        {
            ResizeImage(input);
        }
        else
        {
            DeleteResizeImage();
        }

        return input;
    }

    /// <summary>
    /// Resize image
    /// </summary>
    /// <param name="input">Image input</param>
    private void ResizeImage(UploadImageInput input)
    {
        foreach (var filePath in Directory.GetFiles(this.UploadImageTempPath))
        {
            var fileName = Path.GetFileName(filePath);

            // Move base image before resize to base directory
            EnsureUploadImageTempPathExist(this.UploadImageBasePath);
            var destFileName = Path.Combine(this.UploadImageBasePath, Path.GetFileName(filePath));
            File.Copy(filePath, destFileName, true);

            if (fileName.Contains(Constants.PRODUCTIMAGE_FOOTER_LL))
            {
                var isSubImage = fileName.Contains(Constants.PRODUCTSUBIMAGE_FOOTER);
                input.ResizeImage(filePath, isSubImage);
            }
        }
    }

    /// <summary>
    /// Delete resize image
    /// </summary>
    private void DeleteResizeImage()
    {
        if (Directory.Exists(this.UploadImageBasePath) == false) return;

        foreach (var filePath in Directory.GetFiles(this.UploadImageTempPath))
        {
            var fileName = Path.GetFileName(filePath);

            if (fileName.Contains(Constants.PRODUCTIMAGE_FOOTER_L)
                || fileName.Contains(Constants.PRODUCTIMAGE_FOOTER_M)
                || fileName.Contains(Constants.PRODUCTIMAGE_FOOTER_S))
            {
                DeleteImage(filePath);
            }
            else
            {
                var sourceFileName = Path.Combine(this.UploadImageBasePath, Path.GetFileName(filePath));
                File.Copy(sourceFileName, filePath, true);
            }
        }

        Directory.Delete(this.UploadImageBasePath, true);
    }

    /// <summary>
    /// Update image head
    /// </summary>
    /// <param name="input">Image input</param>
    private void UpdateImageHead(UploadImageInput input)
    {
        var productInput = (ProductInput)SessionManager.Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO];
        input.ImageHead = productInput.ImageHead;

        if (productInput.ProductVariations.Length == 0) return;
        for (var index = 0; index < input.VariationImages.Length; index++)
        {
            input.VariationImages[index].ImageHead = productInput.ProductVariations[index].VariationImageHead;
        }
    }

    /// <summary>
    /// Delete unused images
    /// </summary>
    /// <param name="input">An upload image input</param>
    private void DeleteUnusedImages(UploadImageInput input)
    {
        // Because when user press back button from ProductConfirm the user can change images.
        // Depend on user action it may be create unused images. So these pictures need to be clean up.
        // Delete unused main image
        if (input.MainImage.DelFlg)
        {
            var filePath = Path.Combine(this.UploadImageTempPath, StringUtility.ToEmpty(input.MainImage.FileName));
            DeleteImage(filePath);
            input.MainImage.FileName = string.Empty;
        }

        // Delete unused sub images
        foreach (var subImage in input.SubImages.Where(image => image.DelFlg))
        {
            var filePath = Path.Combine(this.UploadImageTempPath, StringUtility.ToEmpty(subImage.FileName));
            DeleteImage(filePath);
            subImage.FileName = string.Empty;
        }

        // Delete unused variation images
        foreach (var variationImage in input.VariationImages.Where(image => image.DelFlg))
        {
            var filePath = Path.Combine(this.UploadImageTempPath, StringUtility.ToEmpty(variationImage.FileName));
            DeleteImage(filePath);
            variationImage.FileName = string.Empty;
        }
    }

    /// <summary>
    /// Delete image
    /// </summary>
    /// <param name="filePath">File path</param>
    private void DeleteImage(string filePath)
    {
        if (File.Exists(filePath))
        {
            var isReadOnly = new FileInfo(filePath).IsReadOnly;
            if (isReadOnly) File.SetAttributes(filePath, FileAttributes.Normal);

            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Move to rename temporary path
    /// </summary>
    /// <param name="previousFileName">A previous file name</param>
    /// <param name="newFileName">A new file name</param>
    /// <returns>A new file name</returns>
    private string MoveToRenameTempPath(string previousFileName, string newFileName)
    {
        var previousFilePath = Path.Combine(this.UploadImageTempPath, previousFileName);
        var newFilePath = Path.Combine(this.UploadImageRenameTempPath, newFileName);
        File.Move(previousFilePath, newFilePath);

        return newFileName;
    }

    /// <summary>
    /// Rename images
    /// </summary>
    /// <param name="input">Upload image input</param>
    private void RenameImages(UploadImageInput input)
    {
        EnsureUploadImageTempPathExist(this.UploadImageRenameTempPath);

        // Main image
        var previousFileName = input.MainImage.FileName;
        var newFileName = input.ImageHead + Constants.PRODUCTIMAGE_FOOTER_LL;
        if (input.MainImage.HasImage && (previousFileName != newFileName))
        {
            input.MainImage.FileName = MoveToRenameTempPath(previousFileName, newFileName);
        }

        // Sub images
        foreach (var subImage in input.SubImages)
        {
            previousFileName = subImage.FileName;
            newFileName = CreateSubImageFileName(subImage.ImageNo, input.ImageHead);
            if (subImage.HasImage && (previousFileName != newFileName))
            {
                subImage.FileName = MoveToRenameTempPath(previousFileName, newFileName);
            }
        }

        // Move all images from rename temporary path to temporary path
        foreach (var filePath in Directory.GetFiles(this.UploadImageRenameTempPath))
        {
            var destFileName = Path.Combine(this.UploadImageTempPath, Path.GetFileName(filePath));
            File.Move(filePath, destFileName);
        }

        // Remove everything in rename temporary path when done
        Directory.Delete(this.UploadImageRenameTempPath);
    }

    /// <summary>
    /// Create new image temp path
    /// </summary>
    private void CreateNewImageTempPath(string newGuidString)
    {
        var newTempPath = this.UploadImageTempPath.Replace(this.GuidString, newGuidString);
        if (Directory.Exists(newTempPath) == false) Directory.CreateDirectory(newTempPath);
        foreach (var filePath in Directory.GetFiles(this.UploadImageTempPath))
        {
            var destFileName = Path.Combine(newTempPath, Path.GetFileName(filePath));
            File.Copy(filePath, destFileName);
        }

        if (Directory.Exists(this.UploadImageBasePath))
        {
            newTempPath = this.UploadImageBasePath.Replace(this.GuidString, newGuidString);
            Directory.CreateDirectory(newTempPath);
            foreach (var filePath in Directory.GetFiles(this.UploadImageBasePath))
            {
                var destFileName = Path.Combine(newTempPath, Path.GetFileName(filePath));
                File.Copy(filePath, destFileName);
            }
        }
    }

    /// <summary>
    /// Save file
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="destFileName">Destination file name</param>
    /// <param name="fileIndex">File index</param>
    /// <returns>A file</returns>
    private string SaveFile(
        HttpContext context,
        string destFileName,
        int fileIndex)
    {
        try
        {
            // Get file from context
            var file = context.Request.Files.Get(fileIndex);

            // Check file type
            if (GetKnownFileType(file.InputStream) == FileType.Unknown) return string.Empty;

            // Save file temp
            var filePath = Path.Combine(this.UploadImageTempPath, destFileName);
            file.SaveAs(filePath);

            return destFileName;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Ensure upload image temporary path exist
    /// </summary>
    /// <param name="dirPath">Direction path</param>
    private void EnsureUploadImageTempPathExist(string dirPath)
    {
        if (Directory.Exists(dirPath)) return;
        Directory.CreateDirectory(dirPath);
    }

    /// <summary>
    /// Create sub image file name
    /// </summary>
    /// <param name="imageNo">Image no</param>
    /// <param name="imageHead">Image head</param>
    /// <returns>Sub image file name></returns>
    private string CreateSubImageFileName(int imageNo, string imageHead)
    {
        var fileName = string.Format(
            "{0}{1}{2}{3}",
            imageHead,
            Constants.PRODUCTSUBIMAGE_FOOTER,
            imageNo.ToString(Constants.PRODUCTSUBIMAGE_NUMBERFORMAT),
            Constants.PRODUCTIMAGE_FOOTER_LL);
        return fileName;
    }

    /// <summary>
    /// Create sub image search file name pattern
    /// </summary>
    /// <param name="imageNo">Image no</param>
    /// <param name="imageHead">Image head</param>
    /// <returns>Sub image search file name pattern</returns>
    private string CreateSubImageSearchFileNamePattern(int imageNo, string imageHead)
    {
        var fileName = string.Format(
            "{0}{1}{2}{3}",
            imageHead,
            Constants.PRODUCTSUBIMAGE_FOOTER,
            imageNo.ToString(Constants.PRODUCTSUBIMAGE_NUMBERFORMAT),
            Constants.PRODUCTIMAGE_FOOTER);

        return fileName;
    }

    /// <summary>
    /// Validate input
    /// </summary>
    /// <param name="input">Product input</param>
    /// <returns>An error messages</returns>
    private string ValidateInput(ProductInput input)
    {
        var errorMessages = input.Validate(this.ActionKbn);
        if (errorMessages.Count == 0) return string.Empty;

        return ConvertObjectToJsonString(errorMessages);
    }

    /// <summary>
    /// Move to product confirm process
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>A result as json</returns>
    private string MoveToProductConfirmProcess(HttpContext context)
    {
        var input = GetInputFromRequest(context);

        // 定期購入フラグが「不可」の場合、定期購入に関する項目の値をクリアする
        if (input.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
        {
            input.FixedPurchaseLimitedSkippedCount = string.Empty;
            input.FixedPurchasedCancelableCount = string.Empty;
            input.FixedPurchasedLimitedUserLevelIds = string.Empty;
        }

        input.MallExProductId =
            (Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION && string.IsNullOrEmpty(input.MallExProductId))
                ? input.ProductId
                : input.MallExProductId;

        var response = new ProductMasterProcessResponse();
        response.GuidString = this.GuidString;

        // Validate process
        var errorMessages = ValidateInput(input);
        if (string.IsNullOrEmpty(errorMessages) == false)
        {
            response.ErrorMessage = errorMessages;
            return ConvertObjectToJsonString(response);
        }

        // Save product input to session
        input.GuidString = response.GuidString;
        SessionManager.Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO] = input;
        SessionManager.Session[Constants.SESSION_KEY_ACTION_STATUS] = this.ActionKbn;
        SessionManager.Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = 1;

        var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_CONFIRM)
            .AddParam(Constants.REQUEST_KEY_PRODUCT_ID, context.Request.Form[Constants.FIELD_PRODUCT_PRODUCT_ID])
            .AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionKbn)
            .AddParam(Constants.REQUEST_KEY_HIDE_BACK_BUTTON, context.Request.Form[Constants.REQUEST_KEY_HIDE_BACK_BUTTON]);
        response.ConfirmUrl = urlCreator.CreateUrl();
        response.IsSuccess = true;

        return ConvertObjectToJsonString(response);
    }

    /// <summary>
    /// Convert object to json string
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>Json string</returns>
    private string ConvertObjectToJsonString(object data)
    {
        return BasePageHelper.ConvertObjectToJsonString(data);
    }

    /// <summary>
    /// Convert json string to object
    /// </summary>
    /// <typeparam name="TObject">A type of object</typeparam>
    /// <param name="data">Data</param>
    /// <returns>A object</returns>
    private static TObject ConvertJsonStringToObject<TObject>(string data)
        where TObject : new()
    {
        return BasePageHelper.DeserializeJsonObject<TObject>(data);
    }

    /// <summary>
    /// Get known file type
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>File type</returns>
    public static FileType GetKnownFileType(Stream data)
    {
        foreach (var fileHeader in KNOWN_FILE_HEADERS)
        {
            data.Seek(0, SeekOrigin.Begin);

            var slice = new byte[fileHeader.Value.Length];
            data.Read(slice, 0, fileHeader.Value.Length);
            if (slice.SequenceEqual(fileHeader.Value))
            {
                return fileHeader.Key;
            }
        }

        data.Seek(0, SeekOrigin.Begin);
        return FileType.Unknown;
    }

    /// <summary>Action kbn</summary>
    private string ActionKbn
    {
        get { return StringUtility.ToEmpty(HttpContext.Current.Request.Form[Constants.PARAM_PRODUCT_PROCESS_ACTION_KBN]); }
    }
    /// <summary>Logged in shop operator</summary>
    public ShopOperatorModel LoggedInShopOperator
    {
        get
        {
            return (ShopOperatorModel)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR]
                ?? new ShopOperatorModel();
        }
    }
    /// <summary>Is reusable</summary>
    public bool IsReusable { get { return false; } }
    /// <summary>An upload image main path</summary>
    private string UploadImageMainPath
    {
        get
        {
            return Path.Combine(
                Constants.PHYSICALDIRPATH_CONTENTS_ROOT,
                Constants.PATH_PRODUCTIMAGES.Replace("/", @"\"),
                this.LoggedInShopOperator.ShopId + @"\");
        }
    }
    /// <summary>An upload image sub path</summary>
    private string UploadImageSubPath
    {
        get
        {
            return Path.Combine(
                Constants.PHYSICALDIRPATH_CONTENTS_ROOT,
                Constants.PATH_PRODUCTSUBIMAGES.Replace("/", @"\"),
                this.LoggedInShopOperator.ShopId + @"\");
        }
    }
    /// <summary>An upload image temporary path</summary>
    private string UploadImageTempPath
    {
        get
        {
            return Path.Combine(
                Constants.PHYSICALDIRPATH_CONTENTS_ROOT,
                Constants.PATH_TEMP.Replace("/", @"\"),
                "ProductImages",
                this.GuidString);
        }
    }
    /// <summary>An upload image temporary path that use when image need to change the name</summary>
    private string UploadImageRenameTempPath
    {
        get { return Path.Combine(this.UploadImageTempPath, "Temp"); }
    }
    /// <summary>An upload image base path that use when image is change from resize to not resize</summary>
    private string UploadImageBasePath
    {
        get { return Path.Combine(this.UploadImageTempPath, "Base"); }
    }
    /// <summary>Guid</summary>
    private string GuidString
    {
        get
        {
            var guid = HttpContext.Current.Request.Form[Constants.PARAM_GUID_STRING];
            var result = string.IsNullOrEmpty(guid)
                ? string.Format("{0:yyyyMMdd}_{1}", DateTime.Now, Guid.NewGuid())
                : guid;
            return result;
        }
    }
    /// <summary>An upload image input</summary>
    public UploadImageInput UploadImageInput { get; set; }
    /// <summary>Is back from confirm</summary>
    private bool IsBackFromConfirm
    {
        get
        {
            var isBackFromConfirm = HttpContext.Current.Request.Form[Constants.PARAM_IS_BACK_FROM_CONFIRM];
            var result = ObjectUtility.TryParseBool(isBackFromConfirm);
            return result;
        }
    }
}
