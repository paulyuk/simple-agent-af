#!/bin/bash

set -e

if [ ! -f "./local.settings.json" ]; then

    output=$(azd env get-values)

    # Initialize variables
    AIProjectEndpoint=""
    StorageConnectionQueue=""
    ModelDeploymentName=""

    # Parse the output to get the endpoint URLs
    while IFS= read -r line; do
        if [[ $line == *"PROJECT_ENDPOINT"* ]]; then
            AIProjectEndpoint=$(echo "$line" | cut -d '=' -f 2 | tr -d '"')
        fi
        if [[ $line == *"STORAGE_CONNECTION__queueServiceUri"* ]]; then
            StorageConnectionQueue=$(echo "$line" | cut -d '=' -f 2 | tr -d '"')
        fi
        if [[ $line == *"MODEL_DEPLOYMENT_NAME"* ]]; then
            ModelDeploymentName=$(echo "$line" | cut -d '=' -f 2 | tr -d '"')
        fi
    done <<< "$output"

    cat <<EOF > ./local.settings.json
{
    "IsEncrypted": "false",
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
        "PROJECT_ENDPOINT": "$AIProjectEndpoint",
        "MODEL_DEPLOYMENT_NAME": "$ModelDeploymentName",
        "STORAGE_CONNECTION__queueServiceUri": "$StorageConnectionQueue"
    }
}
EOF

fi