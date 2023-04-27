source BuildScripts/variables.sh

getMicroserviceByFolder .

ROOT_WORKING_DIR=$(pwd)

echo "$ROOT_WORKING_DIR"

for service in "${!MICROSERVICE_LIST[@]}";
do
    buildService=$(echo "build_${serivce//./}" | tr "[:lower:]" "[:upper:]")
    
    cd "$ROOT_WORKING_DIR/${MICROSERVICE_LIST[${serivce}]#.\/}"
    publishDir="obj/Docker/publish"
    dotnet pushlish --no-restore --output "$publishDir" > /dev/null

    cd "$ROOT_WORKING_DIR"

done