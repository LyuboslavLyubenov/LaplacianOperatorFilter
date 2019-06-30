# Laplacian operator filter

Filter using my MatrixEssentials project and GaussianFilter project in order to apply laplacian operator filter on image

[](image2.png)
[](result.png)

## Requirements
- .netcore >= 2.2 
- GaussianFilter
- MatrixEssentials

## Building
First restore packages:
```
dotnet restore
```
then build:
```
dotnet build
```

## Usage
```
dotnet run -- [param] [param2] [param3] ...
```

### Parameters

First is input image path
Second is output image path

### Examples: 
```
dotnet run -- "./image.png" "./laplacian.png"
```

### Notes:
It will create 
