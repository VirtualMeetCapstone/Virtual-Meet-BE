namespace GOCAP.Api.Controllers;

[Route("signin")]
public class AuthController(IAuthService _service) : ApiControllerBase
{
    [HttpGet("google")]
    public async Task<ApiResponse> SignInGoogle([FromQuery] string idToken)
    {
        try
        {
            var payload = await _service.VerifyGoogleTokenAsync(idToken);
            var user = await _service.GetOrCreateUserAsync(payload);
            var jwtToken = await _service.GenerateJwtToken(user);
            return new ApiResponse { Data = jwtToken };
        }
        catch (Exception ex)
        {
            throw new ParameterInvalidException(ex.Message);
        }
    }

}

//https://accounts.google.com/o/oauth2/auth?client_id=192713002905-t0n84r06cfn8seolr6u78os85jf276sq.apps.googleusercontent.com&redirect_uri=https://localhost:7035/signin/google-response&response_type=code&scope=openid%20email%20profile&access_type=offline&prompt=consent
//https://www.facebook.com/v10.0/dialog/oauth?client_id=1259288345202083&redirect_uri=https://localhost:7035/signin/facebook-response&response_type=code&scope=email,public_profile
//eyJhbGciOiJSUzI1NiIsImtpZCI6Ijc2M2Y3YzRjZDI2YTFlYjJiMWIzOWE4OGY0NDM0ZDFmNGQ5YTM2OGIiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiI2NTY2NzYzNjkxOTEtZHFwZTZ2YmwzdGR2MjloZWRnMWtsYmUydjFkNzVxcW8uYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiI2NTY2NzYzNjkxOTEtZHFwZTZ2YmwzdGR2MjloZWRnMWtsYmUydjFkNzVxcW8uYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMTU4MjMwODQyNDczODU4NjU3MDIiLCJoZCI6ImZwdC5lZHUudm4iLCJlbWFpbCI6InR1b25nbm1kZTE3MDU3OEBmcHQuZWR1LnZuIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsIm5iZiI6MTc0MDU1MDAwNywibmFtZSI6Ik5ndXllbiBNYW5oIFR1b25nIChLMTcgRE4pIiwicGljdHVyZSI6Imh0dHBzOi8vbGgzLmdvb2dsZXVzZXJjb250ZW50LmNvbS9hL0FDZzhvY0wwLTBzT1BSMkNuN014bHBpZ1lQSnJEN2hRSUpNVVpkSzF0OHZTdjM5LTl6NUhCTGR4PXM5Ni1jIiwiZ2l2ZW5fbmFtZSI6Ik5ndXllbiBNYW5oIFR1b25nIiwiZmFtaWx5X25hbWUiOiIoSzE3IEROKSIsImlhdCI6MTc0MDU1MDMwNywiZXhwIjoxNzQwNTUzOTA3LCJqdGkiOiJmOTRiYmVmZjc1MzJlYjI0YjliYTA1ZTE1MGZhNDVlYTI4ODEwNWJkIn0.ezyzODp3ncJr-ArQrU5p7gIITnsjdGWVZZGv9GqVlK9_0hy_TReiHo0SZk50HY4T_lA7vhXQ1Nal0V6Q0zNLp73aXDsdP-uDRFfuk50vuP-RtNkt1j1MDSPCfYN6T_4oNQiD1tIJEkQte8rkMbajGOU5nDMhE2aTFHOid-aOMkeodclPC0qHoPX2NIwh-bkTY6VnBeQNSukJYIDr5tnsOCy0rAHqIAvB2TSsj83L4gieFc8f2kjaWf4bkQy_lHAT74r8QkNexNsxHr1tUXONS5h-wxIVT2tp2vfKX_XhKVCCS5kfOdzaBdUjw_tUwH5e6ypmzneZ9ekNZkcpoHM6ww