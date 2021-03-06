#!/bin/bash

CUSTOM_SETTINGS_PATH="/opt/raven-settings.json"

cd /opt/RavenDB/Server

if [ ! -f "$CUSTOM_SETTINGS_PATH" ]
then
    ./Raven.Server \
        /Raven/RunAsService=true \
        /Raven/ServerUrl/Tcp=38888 \
        /Raven/AllowAnonymousUserToAccessTheServer=${AllowAnonymousUserToAccessTheServer} \
        /Raven/DataDir=${DataDir} \
        --print-id
else
    ./Raven.Server \
        /Raven/RunAsService=true \
        /Raven/ServerUrl/Tcp=38888 \
        /Raven/Config=${CUSTOM_SETTINGS_PATH} \
        /Raven/DataDir=${DataDir} \
        --print-id
fi