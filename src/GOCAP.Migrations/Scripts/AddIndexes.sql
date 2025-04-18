-- Create indexes in room table.
CREATE INDEX IX_Rooms_Topic ON Rooms(Topic);
CREATE INDEX IX_Rooms_CreateTime ON Rooms(CreateTime DESC);
CREATE INDEX IX_Rooms_OwnerId ON Rooms(OwnerId);

-- Create indexes in room table.
CREATE INDEX IX_RoomMembers_RoomId ON RoomMembers(RoomId);

-- Create indexes in users table.
CREATE INDEX IX_Users_Name ON Users(Name);