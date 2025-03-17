CREATE INDEX IX_Rooms_Topic ON Rooms(Topic);
CREATE INDEX IX_Rooms_CreateTime ON Rooms(CreateTime DESC);
CREATE INDEX IX_Rooms_OwnerId ON Rooms(OwnerId);
CREATE INDEX IX_RoomMembers_RoomId ON RoomMembers(RoomId);

CREATE INDEX IX_Users_Name ON Users(Name);