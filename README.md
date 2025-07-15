# AniAdamPashut/ctfs

When I think of a challenge that will force others to understand a certain part aspect of programming *I* deem important I create a ctf-like challenge for it.

Most of these differ greatly than classic ctfs as they don't require you to use a classic vulnerability (though some might be solved that way) but require you to understand the systems enough to be able to get the flag. I carefully expose the ports for these ctfs so don't question the available resources, use them instead!

# Running 
Most challenges only require to have `docker compose` and use `docker compose up`. If a `docker-compose.yml` does not appear in the directory of the challenges I hope I will remember to create run instructions.

Due to the use of docker images (and the flag existing in them) it is inherently possible to fetch the flags from them use `docker exec` this is never the solution. Don't do that.

# Contibuting
You have a nice challenge idea?
Open an issue and we will have a further discussion about it. Please, do NOT write the actual challenge in the issue.

# License 
All ctfs are licensed under the `GPLv3` that is provided in this repository unless stated otherwise.

