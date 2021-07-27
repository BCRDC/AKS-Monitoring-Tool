openssl req -x509 -nodes -days 3650 -newkey rsa:2048 -keyout ./backend.key -out ./backend.cert

kubectl create secret generic tls-secret \
--from-file=backend.key=./backend.key \
--from-file=backend.cert=./backend.cert \
 -o yaml --dry-run=client --namespace monitoring \
|  kubectl replace|apply -f -