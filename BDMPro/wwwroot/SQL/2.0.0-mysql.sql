﻿
CREATE TABLE ErrorLog (
    Id VARCHAR(128) NOT NULL,
    UserId VARCHAR(128),
    ErrorMessage TEXT NULL,
    ErrorDetails TEXT NULL,
    ErrorDate datetime(6) NULL,
    PRIMARY KEY (Id),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE Country (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name NVARCHAR(128) NULL
);

insert into Country (Name) values 
('Afghanistan'),('Albania'),('Algeria'),('Andorra'),('Angola'),('Antigua and Barbuda'),('Argentina'),('Armenia'),('Australia'),('Austria'),('Azerbaijan'),
('Bahamas'),('Bahrain'),('Bangladesh'),('Barbados'),('Belarus'),('Belgium'),('Belize'),('Benin'),('Bhutan'),('Bolivia'),('Bosnia and Herzegovina'),('Botswana'),('Brazil'),('Brunei'),('Bulgaria'),('Burkina Faso'),('Burundi'),
('Cabo Verde'),('Cambodia'),('Cameroon'),('Canada'),('Central African Republic'),('Chad'),('Channel Islands'),('Chile'),('China'),('Colombia'),
('Comoros'),('Congo'),('Costa Rica'),('Côte d''Ivoire'),('Croatia'),('Cuba'),('Cyprus'),('Czech Republic'),
('Denmark'),('Djibouti'),('Dominica'),('Dominican Republic'),('DR Congo'),('Ecuador'),('Egypt'),('El Salvador'),('Equatorial Guinea'),
('Eritrea'),('Estonia'),('Eswatini'),('Ethiopia'),('Faeroe Islands'),('Fiji'),('Finland'),('France'),('French Guiana'),
('Gabon'),('Gambia'),('Georgia'),('Germany'),('Ghana'),('Gibraltar'),('Greece'),('Greenland'),('Grenada'),('Guadeloupe'),('Guatemala'),('Guinea'),('Guinea-Bissau'),('Guyana'),
('Haiti'),('Holy See'),('Honduras'),('Hong Kong'),('Hungary'),('Iceland'),('India'),('Indonesia'),('Iran'),('Iraq'),('Ireland'),('Isle of Man'),('Israel'),('Italy'),('Jamaica'),('Japan'),('Jordan'),('Kazakhstan'),('Kenya'),('Kiribati'),('Kuwait'),('Kyrgyzstan'),
('Laos'),('Latvia'),('Lebanon'),('Lesotho'),('Liberia'),('Libya'),('Liechtenstein'),('Lithuania'),('Luxembourg'),('Macao'),('Madagascar'),('Malawi'),('Malaysia'),('Maldives'),('Mali'),('Malta'),('Marshall Islands'),('Martinique'),('Mauritania'),('Mauritius'),('Mayotte'),('Mexico'),('Micronesia'),('Moldova'),('Monaco'),('Mongolia'),('Montenegro'),('Morocco'),('Mozambique'),('Myanmar'),
('Namibia'),('Nauru'),('Nepal'),('Netherlands'),('New Caledonia'),('New Zealand'),('Nicaragua'),('Niger'),('Nigeria'),('North Korea'),('North Macedonia'),('Norway'),
('Oman'),('Pakistan'),('Palau'),('Panama'),('Papua New Guinea'),('Paraguay'),('Peru'),('Philippines'),('Poland'),('Portugal'),
('Qatar'),('Réunion'),('Romania'),('Russia'),('Rwanda'),('Saint Helena'),('Saint Kitts and Nevis'),('Saint Lucia'),('Saint Vincent and the Grenadines'),('Samoa'),('San Marino'),('Sao Tome & Principe'),('Saudi Arabia'),('Senegal'),('Serbia'),('Seychelles'),('Sierra Leone'),('Singapore'),('Slovakia'),('Slovenia'),('Solomon Islands'),('Somalia'),('South Africa'),('South Korea'),('South Sudan'),('Spain'),('Sri Lanka'),('State of Palestine'),('Sudan'),('Suriname'),('Sweden'),('Switzerland'),('Syria'),
('Taiwan'),('Tajikistan'),('Tanzania'),('Thailand'),('Timor-Leste'),('Togo'),('Tonga'),('Trinidad and Tobago'),('Tunisia'),('Turkey'),('Turkmenistan'),('Tuvalu'),
('Uganda'),('Ukraine'),('United Arab Emirates'),('United Kingdom'),('United States'),('Uruguay'),('Uzbekistan'),('Vanuatu'),('Venezuela'),
('Vietnam'),('Western Sahara'),('Yemen'),('Zambia'),('Zimbabwe');