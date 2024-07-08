function SocialPlusInputCompletion(profileData, selectors) {
  var mails = profileData['email'];
  if (mails && (mails.length > 0)) {
    var email = mails['0']['email'];
    if (($(selectors['mailAddress']).val() != undefined) && (email != null)) $(selectors['mailAddress']).val(email);
    if (($(selectors['mailAddressConf']).val() != undefined) && (email != null)) $(selectors['mailAddressConf']).val(email);
  }
  var phone = profileData['phone'];
  if (phone && (phone.length > 0)) {
    var number = phone['0']['number'];
    if (number != null) {
      if ($(selectors['tell1']).val() != undefined) {
        var numbercount = number.length === 10 ? 2 : 3;
        $(selectors['tell1']).val(number.slice(0, numbercount));
        $(selectors['tell2']).val(number.slice(numbercount, numbercount + 4));
        $(selectors['tell3']).val(number.slice(numbercount + 4, numbercount + 8));
      } else {
        if ($(selectors['tell']).val() != undefined) $(selectors['tell']).val(number);
      }
    }
  }
  var profile = profileData['profile'];
  if (profile) {
    var lastName = profile['last_name'];
    if (($(selectors['name1']).val() != undefined) && (lastName != null)) $(selectors['name1']).val(lastName);
    var firstName = profile['first_name'];
    if (($(selectors['name2']).val() != undefined) && (firstName != null)) $(selectors['name2']).val(firstName);
    var lastNameKana = profile['last_name_kana'];
    if (($(selectors['nameKana1']) != undefined) && (lastNameKana != null)) $(selectors['nameKana1']).val(hiraganaConversion(lastNameKana));
    var firstNameKana = profile['first_name_kana'];
    if (($(selectors['nameKana2']).val() != undefined) && (firstNameKana != null)) $(selectors['nameKana2']).val(hiraganaConversion(firstNameKana));
    if ($(selectors['productreviewEnabled'])) {
      var userName = profile['user_name'];
      if (($(selectors['nickName']).val() != undefined) && (userName != null)) $(selectors['nickName']).val(userName);
    }
    var birthday = profile['birthday'];
    if (birthday != null) {
      var birthdayList = birthday.split('-');
      if ($(selectors['birthYear']).val() != undefined) $(selectors['birthYear']).val(Number(birthdayList['0']));
      if ($(selectors['birthMonth']).val() != undefined) $(selectors['birthMonth']).val(Number(birthdayList['1']));
      if ($(selectors['birthDay']).val() != undefined) $(selectors['birthDay']).val(Number(birthdayList['2']));
    }
    var gender = profile['gender'];
    var genderNumer = Number(gender);
    if ((genderNumer === 0) || (genderNumer === 3)) $(selectors['sex']).val(['MALE']);
    if (genderNumer === 2) $(selectors['sex']).val(['FEMALE']);
    var postalCode = profile['postal_code'];
    if (postalCode != null) {
      if ($(selectors['zip1']).val() != undefined) {
        $(selectors['zip1']).val(postalCode.slice(0, 3));
        $(selectors['zip2']).val(postalCode.slice(3, 7));
      } else {
        if ($(selectors['zip']).val() != undefined) $(selectors['zip']).val(postalCode);
      }
    }
    var prefecture = profile['prefecture'];
    if (($(selectors['address1']).val() != undefined) && (prefecture != null)) $(selectors['address1']).val(prefecture);
    var city = profile['city'];
    if (($(selectors['address2']).val() != undefined) && (city != null)) $(selectors['address2']).val(city);
    var street = profile['street'];
    if (($(selectors['address3']).val() != undefined) && (street != null)) $(selectors['address3']).val(street);
  }
}

function hiraganaConversion(namekana) {
  if (selectors['nameKanaCheck'] === 'FULLWIDTH_HIRAGANA') {
    namekana = namekana.replace(/['ァ-ン']/g,
      function (s) {
        return String.fromCharCode(s.charCodeAt(0) - 0x60);
      });
  }
  return namekana;
}