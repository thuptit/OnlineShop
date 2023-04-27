set -e

source BuildScripts/variables.sh

getMicroserviceByFolder .

ROOT_WORKING_DIR=$(pwd)

echo "Root folder: $ROOT_WORKING_DIR"

for service in "${MICROSERVICE_LIST[@]}";
do
    echo "name serivce: $service"
    buildService=$(echo "build_${serivce//./}" | tr "[:lower:]" "[:upper:]")
    
    echo "Folder Service: $ROOT_WORKING_DIR/${service#.\/}"

    cd "$ROOT_WORKING_DIR/${service#.\/}"

    publishDir="obj/Docker/publish"

    dotnet build

    dotnet publish --no-restore --output "$publishDir" > /dev/null

    cd "$ROOT_WORKING_DIR"

done

for service in "${MICROSERVICE_LIST[@]}";
do
    
    cd "$ROOT_WORKING_DIR/${service#.\/}"

    str_not_slash=${service//\//}
    str_not_dot=${str_not_slash//./}
    current_datetime=$(date +"%Y%m%d%H%M%S")
    image_name=$str_not_dot$current_datetime
    
    docker build -t "$image_name" . > /dev/null

    cd "$ROOT_WORKING_DIR"

done