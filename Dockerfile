FROM mono:5

COPY . /home/code

WORKDIR /home/code

ARG version_suffix

RUN ./release.sh $version_suffix

#CMD ./test.sh


