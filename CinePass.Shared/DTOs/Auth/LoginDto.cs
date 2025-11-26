using System;
using System.Collections.Generic;
using System.Text;

namespace CinePass.Shared.DTOs.Auth;

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
