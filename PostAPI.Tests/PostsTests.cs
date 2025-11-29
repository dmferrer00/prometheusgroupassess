using NUnit.Framework;
using RestSharp;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Text.Json.Nodes; 

namespace PostAPI.Tests;

public class Tests
{
    private RestClient _client;
    private IConfiguration _config;

    [SetUp]
    public void Setup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        _config = builder.Build();

        var baseUrl = _config["BaseUrl"];
        _client = new RestClient(new RestClientOptions(baseUrl));
    }

    [Test]
    public void getPostsTest()
    {
        var request = new RestRequest("/posts/1", Method.Get);
        var response = _client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

        var post = JsonNode.Parse(response.Content);
        Assert.That(post, Is.Not.Null);
        Assert.That(post["id"]?.GetValue<int>(), Is.EqualTo(1));
    }

    [Test]
    public void newPostsTest()
    {
        var request = new RestRequest("/posts", Method.Post);
        var postData = new { title = "Associate QA", body = "Test body content", userId = 5 };
        request.AddJsonBody(postData);
        var response = _client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created));
        Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

        var createdPost = JsonNode.Parse(response.Content);
        Assert.That(createdPost, Is.Not.Null);
        Assert.That(createdPost["title"]?.GetValue<string>(), Is.EqualTo("Associate QA"));
    }

    [Test]
    public void changePosts()
    {
        var request = new RestRequest("/posts/1", Method.Put);
        var postData = new { id = 1, title = "SDET", body = "The body has been updated.", userId = 1 };
        request.AddJsonBody(postData);
        var response = _client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

        var updatedPost = JsonNode.Parse(response.Content);
        Assert.That(updatedPost, Is.Not.Null);
        Assert.That(updatedPost["title"]?.GetValue<string>(), Is.EqualTo("SDET"));
    }

    [Test] 
    public void deletePostsTest()
    {
        var request = new RestRequest("/posts/1", Method.Delete);
        var response = _client.Execute(request);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
    }

    [Test]
    public void getNegativePostsTest()
    {
        var request = new RestRequest("/posts/-1", Method.Get);
        var response = _client.Execute(request);
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }

    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
    }
}