■プロジェクトに新たにSQLのXMLを追加するとき
■Khi Add Thêm XML mới  cùa SQL vào project 

１．VisualStudioから、w2Domainの「\_Repository\」配下にxml新規作成する
１．Từ VisualStudio tạo mới xml trong 「\_Repository\」của w2Domain


２．VisualStudioで一度プロジェクトをビルドすると、「１」で追加したファイルがw2Domainの「\bin\Repository_Release\」配下にコピーされる。
	（このディレクトリはソース管理されていないため、エクスプローラー上で確認可能）
２．Nếu build project 1 lần bằng VisualStudio thì cái file đã thêm vào ở 「１」sẽ được copy vào 「\bin\Repository_Release\」 của w2Domain .
   (Vì cái directory này không được quản lý source nên có thể confirm được bằng Explorer)


３．上記コピーされたファイル「\bin\Repository_Release\***.xml」をリソースに追加する。
    （w2Domainプロジェクトの「プロパティ」→「リソース」タブを開き、該当XMLファイルを追加）
３．Add file được copy ở trên 「\bin\Repository_Release\***.xml」 vào resource .
       (Open Property của Project w2Domain → tab「resource」rồi thêm file xml đó vào) 


４．ここでもし ソリューションエクスプローラー上に追加した「\bin\Repository_Release\***.xml」が
    w2Domain配下に追加されたら、これはソース管理したくないため、「bin」ごとプロジェクトから削除する。
	（ソリューションエクスプローラー上で「w2Domain\bin」を選択し、DELキーで削除。エクスプローラーからは削除しないでください。）
４．Ở đây Nếu 「\bin\Repository_Release\***.xml」 đã add vào solution Explorer mà đã được add vào trong w2Domain rồi thì cái này sẽ không có quản lý source nữa nên
        sẽ xóa [bin] ra khỏi project .
         (Chọn 「w2Domain\bin」trong solution Explorer rồi xóa bằng cách nhấn phím [DEL]. đừng có xóa từ Explorer nhé). 
