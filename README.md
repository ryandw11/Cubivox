![An image of the name "Cubivox" being made out of the game's voxels.](https://img.ryandw11.com/raw/remi3lqas.png)
# Cubivox
Cubivox is an online multiplayer 3D voxel sandbox game developed in C# and Unity. Cubivox allows users to create their own worlds and minigames through the use of a robust and dedicated modding API.

## [Cubivox Documentation](https://docs.ryandw11.com/cubivox/cubivox)

## About Cubivox
Cubivox is divided into 3 core compontents:
- Cubivox Client
  - This repository. This holds the client code and is responsible for the user interface of the game. 
- [Cubivox Server](https://github.com/ryandw11/CubivoxServer)
  - Holds the code for the Cubivox server. Almost all of the game's logic is done on the server, so a server is required to play Cubivox.
- [Cubivox Core](https://github.com/ryandw11/CubivoxCore)
  - The modding API for Cubivox. It is designed to allow for a mod to run both on clients and the server without multiple code bases. See the [documentation](https://docs.ryandw11.com/cubivox/mods/) for more information about modding Cubivox.
