using System.Security;
using AwesomeAssertions;
using CrownsGuard.Multiplayer.Utilities;
using JetBrains.Annotations;

namespace CrownsGuard.Multiplayer.Test.Utilities;

[TestSubject(typeof(HashingHelper))]
public class HashingHelperTest
{
    [Fact]
    public void GetSalt_ReturnsDifferentValues()
    {
        // Arrange & Act
        var salt1 = HashingHelper.GetSalt();
        var salt2 = HashingHelper.GetSalt();
        
        // Assert
        salt1.Should().NotBe(salt2);   
    }

    [Fact]
    public void GetEmailHash_ShouldBeStable()
    {
        // Arrange & Act
        var hash1 = HashingHelper.GetEmailHash("testEmail@test.test");
        var hash2 = HashingHelper.GetEmailHash("testEmail@test.test");
        
        // Assert
        hash1.Should().Be(hash2);  
    }

    [Fact]
    public void GetEmailHash_ShouldReturnDifferentSaltForDifferentValues()
    {
        // Arrange & Act
        var hash1 = HashingHelper.GetEmailHash("TestEmail@test.test");
        var hash2 = HashingHelper.GetEmailHash("testEmail@test.test");
        
        // Assert
        hash1.Should().NotBe(hash2); 
    }

    [Fact]
    public void GetHash_ForSameTextAndSalt_ReturnsSameValue()
    {
        // Arrange & Act
        var salt = HashingHelper.GetSalt();
        var hash1 = HashingHelper.GetHash("test", salt);
        var hash2 = HashingHelper.GetHash("test", salt);
        
        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void GetHash_ForDifferentSalt_ReturnsDifferentValue()
    {
        // Arrange & Act
        var salt1 = HashingHelper.GetSalt();
        var salt2 = HashingHelper.GetSalt();
        var hash1 = HashingHelper.GetHash("test", salt1);
        var hash2 = HashingHelper.GetHash("test", salt2);
        
        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void GetHash_ForDifferentTextSameSalt_ReturnsDifferentValue()
    {
        // Arrange & Act
        var salt = HashingHelper.GetSalt();
        var hash1 = HashingHelper.GetHash("test1", salt);
        var hash2 = HashingHelper.GetHash("test2", salt);
        
        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void GetHashSecureString_ForSameTextAndSalt_ReturnsSameValue()
    {
        // Arrange & Act
        var salt = HashingHelper.GetSalt();
        var secureString1 = new SecureString();
        var secureString2 = new SecureString();
        
        foreach (var c in "test")
        {
            secureString1.AppendChar(c);
            secureString2.AppendChar(c);       
        }
        
        var hash1 = HashingHelper.GetHash(secureString1, salt);
        var hash2 = HashingHelper.GetHash(secureString2, salt);
        
        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void GetHashSecureString_ForSameTextDifferentSalt_ReturnsDifferentValue()
    {
        // Arrange & Act
        var salt1 = HashingHelper.GetSalt();
        var salt2 = HashingHelper.GetSalt();
        var secureString1 = new SecureString();
        var secureString2 = new SecureString();
        
        foreach (var c in "test")
        {
            secureString1.AppendChar(c);
            secureString2.AppendChar(c);       
        }

        var hash1 = HashingHelper.GetHash(secureString1, salt1);
        var hash2 = HashingHelper.GetHash(secureString2, salt2);
        
        // Assert
        hash1.Should().NotBe(hash2);   
    }

    [Fact]
    public void GetHashSecureString_ForDifferentTextSameSalt_ReturnsDifferentValue()
    {
        // Arrange & Act
        var salt = HashingHelper.GetSalt();
        var secureString1 = new SecureString();
        var secureString2 = new SecureString();
        
        foreach (var c in "test1")
        {
            secureString1.AppendChar(c);
        }
        
        foreach (var c in "test2")
        {
            secureString2.AppendChar(c);       
        }
        
        var hash1 = HashingHelper.GetHash(secureString1, salt);
        var hash2 = HashingHelper.GetHash(secureString2, salt);
        
        // Assert
        hash1.Should().NotBe(hash2);
    }
}