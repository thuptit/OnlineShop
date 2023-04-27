set -e

source BuildScripts/variables.sh

docker login docker-registry.hopto.org -u Thutrang2709 -p Thutrang2709

for service in "${MICROSERVICE_LIST[@]}";
do
    str_not_slash=${service//\//}
    str_not_dot=${str_not_slash//./}
    image_name=${str_not_dot,,}
    docker tag "$image_name" docker-registry.hopto.org/"$image_name":1.0 > /dev/nul
    docker push docker-registry.hopto.org/"$image_name":1.0 > /dev/nul
done