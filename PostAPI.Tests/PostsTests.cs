using NUnit.Framework;
using RestSharp;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using PostAPI.Tests.Models;

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
        
        var post = _client.Execute<Post>(request).Data;

        Assert.That(post, Is.Not.Null);
        Assert.That(post.Id, Is.EqualTo(1));
    }

    [Test]
    public void newPostsTest()
    {
        var request = new RestRequest("/posts", Method.Post);
        var newPost = new Post { Title = "Associate QA", Body = "Test body content", UserId = 5 };
        request.AddJsonBody(newPost);
        
        var createdPost = _client.Execute<Post>(request).Data;

        Assert.That(createdPost, Is.Not.Null);
        Assert.That(createdPost.Title, Is.EqualTo(newPost.Title));
    }

    [Test]
    public void changePosts()
    {
        var request = new RestRequest("/posts/1", Method.Put);
        var updatedPostData = new Post { Id = 1, Title = "SDET", Body = "The body has been updated.", UserId = 1 };
        request.AddJsonBody(updatedPostData);
        
        var updatedPostResponse = _client.Execute<Post>(request).Data;

        Assert.That(updatedPostResponse, Is.Not.Null);
        Assert.That(updatedPostResponse.Title, Is.EqualTo(updatedPostData.Title));
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