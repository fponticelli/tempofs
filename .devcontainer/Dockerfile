FROM fsharp

# Copy endpoint specific user settings into container to specify
# .NET Core should be used as the runtime.
COPY settings.vscode.json /root/.vscode-remote/data/Machine/settings.json

ENV DOTNET_VERSION=5.0

RUN apt-get update
RUN apt-get install -y gpg wget git procps curl

RUN wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN apt-get update; \
  apt-get install -y apt-transport-https && \
  apt-get update && \
  apt-get install -y dotnet-sdk-${DOTNET_VERSION}

RUN curl -sL https://deb.nodesource.com/setup_14.x | bash -
RUN apt-get install -y nodejs build-essential
