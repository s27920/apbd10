﻿namespace AuthExample.Dtos.User;

public class TokenResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}