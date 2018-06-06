FROM mono:5

COPY . /home/code

WORKDIR /home/code

ARG version-suffix

RUN ./release.sh $version-suffix

#CMD ./test.sh


