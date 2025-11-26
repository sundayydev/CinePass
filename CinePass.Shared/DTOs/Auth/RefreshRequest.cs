using System;
using System.Collections.Generic;
using System.Text;

namespace CinePass.Shared.DTOs.Auth;

public class RefreshRequest
{
    public string RefreshToken { get; set; }
}
