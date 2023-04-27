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

    # dotnet publish --no-restore --output "$publishDir" > /dev/null

    cd "$ROOT_WORKING_DIR"

done

for service in "${MICROSERVICE_LIST[@]}";
do
    buildService=$(echo "build_${serivce//./}" | tr "[:lower:]" "[:upper:]")
    
    cd "$ROOT_WORKING_DIR/${service#.\/}"

    name=$(echo "$serivce" | tr '[:upper:]' '[:lower:]')

    docker build -t "onlineshop.$name.api" . > /dev/null

    cd "$ROOT_WORKING_DIR"

done