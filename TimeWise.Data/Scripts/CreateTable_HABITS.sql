-- Script SQL para criar a tabela HABITS no Oracle
-- Execute este script diretamente no Oracle SQL Developer ou SQL*Plus

CREATE TABLE HABITS (
    Id RAW(16) NOT NULL,
    UsuarioId RAW(16) NOT NULL,
    Titulo NVARCHAR2(200) NOT NULL,
    Descricao NVARCHAR2(1000),
    Tipo NVARCHAR2(50) NOT NULL,
    CriadoEm TIMESTAMP(7) NOT NULL,
    Concluido NUMBER(1) NOT NULL,
    CONSTRAINT PK_HABITS PRIMARY KEY (Id)
);

-- Comentários nas colunas
COMMENT ON COLUMN HABITS.Id IS 'ID único do hábito (GUID)';
COMMENT ON COLUMN HABITS.UsuarioId IS 'ID do usuário proprietário do hábito';
COMMENT ON COLUMN HABITS.Titulo IS 'Título do hábito';
COMMENT ON COLUMN HABITS.Descricao IS 'Descrição detalhada do hábito';
COMMENT ON COLUMN HABITS.Tipo IS 'Tipo do hábito (PAUSA, POSTURA, HIDRATACAO)';
COMMENT ON COLUMN HABITS.CriadoEm IS 'Data e hora de criação do hábito';
COMMENT ON COLUMN HABITS.Concluido IS 'Indica se o hábito foi concluído (0 = false, 1 = true)';

