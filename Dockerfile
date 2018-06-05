FROM mono:5

COPY . /home/code

WORKDIR /home/code

RUN ./release.sh




