FROM archlinux:base-devel

RUN sudo pacman -Sy less git --needed --noconfirm

WORKDIR /code

ADD . .
RUN rm .gitignore

ENTRYPOINT [ "bash", "startup.sh" ]
