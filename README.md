# DotNetDemo
This app implements the instrumentation examples from the [Prometheus SDK](https://github.com/prometheus-net/prometheus-net) using the ASP.NET Core Web API starting template app in Visual Studio.

To use the same starting app:
1. Open Visual Studio 2022.
2. Choose `Create a new project` -> `ASP.NET Core Web API`.
3. Choose `.NET 7.0` as the framework.
4. Check the boxes for `Enable Docker` and `Use controllers`.
5. Click `Create`.

To deploy the app on your cluster:
1. Build the Docker container image and push to an Azure Container Registry.
2. Replace the value of `image` field in the file `Kubernetes/example-app.yaml` with your image.
3. Make sure your cluster has access to the ACR, and use `kubectl apply -f Kubernetes/example-app.yaml` to deploy onto the cluster.

To scrape the metrics:
1. Make sure the Managed Prometheus addon is enabled.
2. Configure the scrape config either by applying `Kubernetes/ama-metrics-prometheus-config.yaml`, or if using the operator private preview, apply the pod monitor with `Kubernetes/example-podmonitor.yaml`.
