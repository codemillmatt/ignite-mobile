GOOGLE_JSON_FILE=$APPCENTER_SOURCE_DIRECTORY/TailwindTraders.Mobile/TailwindTraders.Mobile/TailwindTraders.Mobile.Android/google-services.json

if [ -e "$GOOGLE_JSON_FILE" ]
then
    echo "Updating Google Json"
    echo "$GOOGLE_JSON" > $GOOGLE_JSON_FILE
    sed -i -e 's/\\"/'\"'/g' $GOOGLE_JSON_FILE

    echo "File content:"
    cat $GOOGLE_JSON_FILE
fi

if [ ! -n "$ANDROID_APPCENTER_SECRET" ] || [ ! -n "$IOS_APPCENTER_SECRET" ]  ; then
	echo ###################################################################################
    echo "Not all needed variables are set. The app won't be auto-provisioned correctly"
	echo ###################################################################################
    exit
fi

ANDROID_APPCENTER_SECRET_PLACEHOLDER="< ENTER YOUR APP CENTER ANDROID SECRET HERE >"
IOS_APPCENTER_SECRET_PLACEHOLDER="< ENTER YOUR APP CENTER IOS SECRET HERE >"

APPCENTER_CONSTANT_FILE=$APPCENTER_SOURCE_DIRECTORY/TailwindTraders.Mobile/TailwindTraders.Mobile/TailwindTraders.Mobile/Helpers/AppCenterConstants.cs

if [ -e "$APP_CONSTANT_FILE" ]
then
    echo "Updating the Android App Center Secrets file"
    sed -i ".bak" -e "s,$ANDROID_APPCENTER_SECRET_PLACEHOLDER,$ANDROID_APPCENTER_SECRET,g" $APPCENTER_CONSTANT_FILE

    echo "Updating iOS AppCenter secret"
	sed -i ".bak" -e "s,$IOS_APPCENTER_SECRET_PLACEHOLDER,$IOS_APPCENTER_SECRET,g" $APPCENTER_CONSTANT_FILE

    echo "File content:"
    cat $APPCENTER_CONSTANT_FILE
fi