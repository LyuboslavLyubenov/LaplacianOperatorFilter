# Laplacian operator filter

Filter using my MatrixEssentials project and GaussianFilter project in order to apply laplacian operator filter on image
Before: 

<img src="image2.jpg" width="300"/>

After:

<img src="result.png" width="300"/>

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
