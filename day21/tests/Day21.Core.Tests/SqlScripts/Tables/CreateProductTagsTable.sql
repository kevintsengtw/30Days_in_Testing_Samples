IF
NOT EXISTS (SELECT * FROM sysobjects WHERE name='ProductTags' AND xtype='U')
CREATE TABLE ProductTags
(
    ProductId INT NOT NULL,
    TagId     INT NOT NULL,
    PRIMARY KEY (ProductId, TagId),
    FOREIGN KEY (ProductId) REFERENCES Products (Id) ON DELETE CASCADE,
    FOREIGN KEY (TagId) REFERENCES Tags (Id) ON DELETE CASCADE
)
