### omlet

* integrates nancy and json-schema

##### enable omlet in nancy

``` csharp
public class Bootstrapper : DefaultNancyBootstrapper
{
  protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
  {
    OmletSchema.Enable(container, pipelines);
    base.ApplicationStartup(container, pipelines);
  }
}
```

##### define schema for a route

``` csharp
public class UsersModule : NancyModule
{
  public UsersModule()
  {
    Get["/users/search"] = this.WithSchema("/schemas/users-search", x => SearchUsers(x.name));
  }
  
  private Response SearchUsers(string name)
  {
    return HttpStatusCode.OK;
  }
}
```

##### define custom schema handler

``` csharp
public class SchemaHandler : ISchemaHandler
{
  public Response OnBrokenRequest(NancyContext context, Request request, IResponseFormatter formatter, ICollection<JsonSchemaMessage> violations)
  {
    var result = new
    {
      errors = violations.Select(x => $"{x.Path}: {x.Message}").ToArray()
    };
  
    return formatter.AsJson(result, HttpStatusCode.BadRequest);
  }
}
```
