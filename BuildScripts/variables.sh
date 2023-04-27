set -e

declare -A MICROSERVICE_LIST #

getMicroserviceByFolder() {
    for item in $1/*; do
        
        if [[ -d $item && $item == *".Api" ]]; then
            echo "root: $1"
            folderName=${1##*/}
            MICROSERVICES+=("$folderName")&&MICROSERVICE_LIST+=([$item]="$item")
            echo "$item - $folderName"
        fi

        if [ -d $item ]; then
            getMicroserviceByFolder $item
        fi
    done
}

getMicroservices() {
    workingDirWithoutEndSlash=${1%/}
    echo "${1%/}"
    for dir in "${workingDirWithoutEndSlash}"/*/
    do
        dirNameWorkingDirWithoutEndSlash=${dir%/}
        dirName=${dirNameWorkingDirWithoutEndSlash##*/}

        MICROSERVICES+=("$dirName")
    done
}

export MICROSERVICE_LIST
export MICROSERVICES=()
export -f getMicroserviceByFolder
export -f getMicroservices