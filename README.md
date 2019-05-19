# Distancify.Migrations.Litium

This project contains a fluid API to aid in generating data in Litium together with Distancify.Migrations.

## Getting Started

### Install

```
Install-Package Distancify.Migrations.Litium
```

### Using

The library revolves around a set of ISeed implementation. Each ISeed implementation provides a method called `Ensure`. This method __ensures__ that the given entity exists in Litium. If the entity already exist, nothing happens. A seed can also verify given properties of the entity.

Example usage:

```csharp
ChannelSeed.Ensure("MyChannel", "DefaultChannelFieldTemplate")
	.WithMarketId("B2C")
	.WithField("Flag", "sv.png")
	.Commit();
```

_Note_ the last method call `Commit()`. This method creates/updates the entity. Nothing will happen if you forget this call.

## Running the tests

The tests are built using xUnit and does not require any setup in order to run inside Visual Studio's standard test runner.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning.

## Publishing

Use CreateRelease.ps1 to create a new release. There's a CI build from every __master__ build on Distancify's internal NuGet feed.

## Authors

See the list of [contributors](https://github.com/distancify/Distancify.Migrations.Litium/graphs/contributors) who participated in this project.

## License

This project is licensed under the LGPL v3 License - see the [LICENSE](LICENSE) file for details