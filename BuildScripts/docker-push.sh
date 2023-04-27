set -e

source BuildScripts/variables.sh

for service in "${MICROSERVICE_LIST[@]}";
do
    str_not_slash=${service//\//}
    str_not_dot=${str_not_slash//./}
    image_name=${str_not_dot,,}
    docker tag "$image_name" --insecure-registry=103.148.57.56/"$image_name":1.0 > /dev/nul
    docker push --insecure-registry=103.148.57.56/"$image_name":1.0 > /dev/nul
done