set -e

source BuildScripts/variables.sh

getMicroserviceByFolder .

for service in "${MICROSERVICE_LIST[@]}";
do
    str_not_slash=${service//\//}
    str_not_dot=${str_not_slash//./}
    image_name=${str_not_dot,,}
    echo "$image_name"
    docker tag $image_name:latest thudevdockerv1/$image_name:1.0 > /dev/nul
    docker push thudevdockerv1/"$image_name":1.0 > /dev/nul
done