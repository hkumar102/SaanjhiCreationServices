#!/bin/bash

echo "🧹 Removing old Shared projects..."

rm -rf Shared/Shared.Domain
rm -rf Shared/Shared.Authentication
rm -rf Contracts

echo "✅ Old Shared projects removed."

echo "📁 Creating new Shared layout..."

mkdir -p Shared/Domain/Entities
mkdir -p Shared/Authentication
mkdir -p Shared/Contracts

# Sample file to avoid empty folders (optional)
touch Shared/Domain/.keep
touch Shared/Authentication/.keep
touch Shared/Contracts/.keep

echo "📦 Creating Shared.csproj..."

cat > Shared/Shared.csproj <<EOF
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="FluentValidation" Version="11.8.0" />
  </ItemGroup>

</Project>
EOF

# Add to solution
echo "➕ Adding Shared.csproj to solution..."
dotnet sln add Shared/Shared.csproj

# Reference it in each *.Application project
echo "🔗 Linking Shared.csproj to each service's Application project..."

for proj in ./services/*Service/*Application/*.csproj; do
  echo "→ Linking to: $proj"
  dotnet add "$proj" reference Shared/Shared.csproj
done

echo
echo "✅ Shared project created and wired up!"
