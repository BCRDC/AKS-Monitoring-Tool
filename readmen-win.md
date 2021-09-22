## set env from windows powershell
$env:KUBECONFIG="C:\kubectl\.kube_prod\config"


## set env from windows bat
set KUBECONFIG="C:\kubectl\.kube_prod\config"


## to get service account in your namespace
kubectl get ServiceAccount  -n monitoring