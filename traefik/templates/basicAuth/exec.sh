htpasswd -nb user password  >> users.txt


kubectl create secret generic auth-secret \
--from-file=users=./users.auth \
 -o yaml --dry-run=client --namespace monitoring \
|  kubectl replace|apply -f -