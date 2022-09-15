FROM ubuntu:22.04 AS ffmpeg-ubuntu
WORKDIR /tmp/ffmpeg

RUN apt-get update
RUN apt-get -y install make nasm gcc mingw-w64 wget bzip2

ARG FFMPEG_VERSION=5.0.1
RUN wget -O- http://ffmpeg.org/releases/ffmpeg-${FFMPEG_VERSION}.tar.bz2 > ffmpeg.tar.bz2
RUN tar -jx --strip-components=1 -f ffmpeg.tar.bz2

RUN ./configure \
    --arch=x86 \
    --target-os=mingw32 \
    --cross-prefix=i686-w64-mingw32- \
    --prefix=/mnt/d/src/ffmpeg/build.d/win/x64 \
    --disable-everything \
    --disable-network \
    --disable-autodetect \
    --enable-decoder=mp3*,pcm*,wav \
    --enable-encoder=wav,pcm_mulaw \
    --enable-parser=mpegaudio \
    --enable-demuxer=mp3,wav \
    --enable-muxer=wav \
    --enable-filter=aresample \
    --enable-protocol=file,pipe

RUN make -j`nproc` -s

FROM alpine:3.15 as ffmpeg-alpine
WORKDIR /tmp/ffmpeg

RUN apk add --no-cache --update libgcc libstdc++ ca-certificates libcrypto1.1 libssl1.1 libgomp expat git
RUN     buildDeps="autoconf \
                   automake \
                   bash \
                   binutils \
                   bzip2 \
                   cmake \
                   coreutils \
                   curl \
                   diffutils \
                   expat-dev \
                   file \
                   g++ \
                   gcc \
                   gperf \
                   libtool \
                   make \
                   nasm \
                   openssl-dev \
                   python3 \
                   tar \
                   xcb-proto \
                   yasm \
                   zlib-dev" && \
        apk add --no-cache --update ${buildDeps}

ARG FFMPEG_VERSION=5.0.1
RUN wget -O- http://ffmpeg.org/releases/ffmpeg-${FFMPEG_VERSION}.tar.bz2 > ffmpeg.tar.bz2
RUN tar -jx --strip-components=1 -f ffmpeg.tar.bz2
RUN ./configure \
    --disable-everything \
    --disable-network \
    --disable-autodetect \
    --enable-decoder=mp3*,pcm*,wav \
    --enable-encoder=wav,pcm_mulaw \
    --enable-parser=mpegaudio \
    --enable-demuxer=mp3,wav \
    --enable-muxer=wav \
    --enable-filter=aresample \
    --enable-protocol=file,pipe
RUN make

FROM scratch AS export-stage
COPY --from=ffmpeg-alpine /tmp/ffmpeg/ffmpeg .
COPY --from=ffmpeg-alpine /tmp/ffmpeg/ffprobe .
COPY --from=ffmpeg-ubuntu /tmp/ffmpeg/ffmpeg.exe .
COPY --from=ffmpeg-ubuntu /tmp/ffmpeg/ffprobe.exe .
