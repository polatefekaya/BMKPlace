using System;
using System.Security.Cryptography;
using System.Text;
using BMKPlace.Application.Contracts.Abstractions.Authentication;
using BMKPlace.Infrastructure.Common.Exceptions;
using BMKPlace.Infrastructure.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BMKPlace.Infrastructure.Authentication;

public class OtpService : IOtpService
{
    private readonly IDistributedCache _cache;
    private readonly OtpOptions _otpOptions;
    private readonly ILogger<OtpService> _logger;
    private const string OtpCachePrefix = "otp:";

    public OtpService(IDistributedCache cache, IOptionsSnapshot<OtpOptions> otpOptionsSnapshot, ILogger<OtpService> logger){
        _cache = cache;
        _otpOptions = otpOptionsSnapshot.Value;
        _logger = logger;
    }

    public async Task<string> GenerateAndStoreOtpAsync(string identifier, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);

        string otp = GenerateNumericOtp(_otpOptions.Length);
        string cacheKey = GetCacheKey(identifier);

        _logger.LogInformation("Generated OTP {OtpLength} digits for identifier {Identifier}", _otpOptions.Length, identifier); 

        DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_otpOptions.ExpiryMinutes)
        };

        byte[] otpBytes = Encoding.UTF8.GetBytes(otp);

        try
        {
            await _cache.SetAsync(cacheKey, otpBytes, cacheOptions, cancellationToken);
            _logger.LogDebug("Stored OTP for {Identifier} in cache with expiry {ExpiryMinutes} minutes.", identifier, _otpOptions.ExpiryMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store OTP in cache for identifier {Identifier}", identifier);
            throw new FailedToStoreOtpException("Failed to store OTP.", ex);
        }

        return otp;
    }

    public async Task<bool> VerifyOtpAsync(string identifier, string otp, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        ArgumentException.ThrowIfNullOrWhiteSpace(otp);

        string cacheKey = GetCacheKey(identifier);

        try
        {
            byte[]? storedOtpBytes = await _cache.GetAsync(cacheKey, cancellationToken);

            if (storedOtpBytes is null || storedOtpBytes.Length == 0)
            {
                _logger.LogWarning("Verification failed: No OTP found in cache for identifier {Identifier}. It might have expired or never existed.", identifier);
                return false; // OTP not found or expired
            }

            string storedOtp = Encoding.UTF8.GetString(storedOtpBytes);

            if (storedOtp == otp)
            {
                _logger.LogInformation("OTP verification successful for identifier {Identifier}.", identifier);

                await _cache.RemoveAsync(cacheKey, cancellationToken);
                _logger.LogDebug("Consumed OTP from cache for {Identifier}.", identifier);
                return true;
            }
            else
            {
                _logger.LogWarning("OTP verification failed: Provided OTP does not match stored OTP for identifier {Identifier}.", identifier);
                // Optional: Implement rate limiting or lockout logic here for repeated failures
                return false; // OTP does not match
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during OTP verification for identifier {Identifier}", identifier);
            // Treat errors as verification failure for security
            return false;
        }
    }

    private static string GenerateNumericOtp(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "OTP length must be positive.");
        }

        byte[] randomBytes = RandomNumberGenerator.GetBytes(length);
        char[] otpChars = new char[length];
        const string digits = "0123456789";

        for (int i = 0; i < length; i++)
        {
            // Map random byte to a digit 0-9
            otpChars[i] = digits[randomBytes[i] % digits.Length];
        }

        return new string(otpChars);
    }

    private static string GetCacheKey(string identifier)
    {
        return $"{OtpCachePrefix}{identifier.ToLowerInvariant().Trim()}";
    }
}
