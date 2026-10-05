create table CRSVEDMQ_XML (
    EnvelopeID VARCHAR2(36),
    DocumentID VARCHAR2(36),
    DocumentDateTime DATE,
    DocNumber VARCHAR2(50),
    CustDocNumber VARCHAR2(50),
    INN VARCHAR2(12),
    ObligationCode VARCHAR2(6)
);
--grant select, insert, update, delete on CRSVEDMQ_XML to OIV_CRSVED_CONNECT;
--create or replace synonym CRSVEDMQ_XML for OIV_CRSVED.CRSVEDMQ_XML;