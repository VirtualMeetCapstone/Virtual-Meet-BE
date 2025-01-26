namespace GOCAP.Common;

public enum ErrorCode
{
    Unauthorized = 10001,
    InvalidAuthorizationToken = 10002,
    ExpiredAuthorizationToken = 10003,
    ResourceNotFound = 10004,
    ResourceDuplicated = 10005,
    OutofUserSeats = 10010,
    InvalidParameters = 20001,
    InvalidTenantID = 20002,
    InvalidTenantIDandApplicationType = 20003,
    InvalidApplicationType = 20004,
    InternalError = 30001,
    CorruptedTenant = 40001,
}

//10001 – No Authorization Token
//10002 – Invalid Authorization Token
//10003 – Expired Authorization Token
//10010 – Out of User Seats
//20001 – Invalid Parameters
//20002 – Invalid Tenant ID
//20003 – Invalid Tenant ID and Application Type
//20004 – Invalid Application Type
//30001 – Internal Error
//40001 - Tenant Not Init
