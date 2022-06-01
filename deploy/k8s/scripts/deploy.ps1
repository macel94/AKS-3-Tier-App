helm upgrade nginx ingress-nginx/ingress-nginx --install
helm upgrade mycustomapp ./../helm --install

# helm uninstall mycustomapp
# helm uninstall nginx