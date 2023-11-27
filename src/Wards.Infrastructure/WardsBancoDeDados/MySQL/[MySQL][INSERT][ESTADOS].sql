SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

-- --------------------------------------------------------

--
-- Estrutura da tabela `Estados`
--

CREATE TABLE `Estados` (
  `EstadoId` int(11) NOT NULL,
  `Nome` longtext NOT NULL,
  `Sigla` longtext NOT NULL,
  `IsAtivo` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Extraindo dados da tabela `Estados`
--

INSERT INTO `Estados` (`EstadoId`, `Nome`, `Sigla`, `IsAtivo`) VALUES
(1, 'Acre', 'AC', 1),
(2, 'Alagoas', 'AL', 1),
(3, 'Amapá', 'AP', 1),
(4, 'Amazonas', 'AM', 1),
(5, 'Bahia', 'BA', 1),
(6, 'Ceará', 'CE', 1),
(7, 'Espírito Santo', 'ES', 1),
(8, 'Goiás', 'GO', 1),
(9, 'Maranhão', 'MA', 1),
(10, 'Mato Grosso', 'MT', 1),
(11, 'Mato Grosso do Sul', 'MS', 1),
(12, 'Minas Gerais', 'MG', 1),
(13, 'Pará', 'PA', 1),
(14, 'Paraíba', 'PB', 1),
(15, 'Paraná', 'PR', 1),
(16, 'Pernambuco', 'PE', 1),
(17, 'Piauí', 'PI', 1),
(18, 'Rio de Janeiro', 'RJ', 1),
(19, 'Rio Grande do Norte', 'RN', 1),
(20, 'Rio Grande do Sul', 'RS', 1),
(21, 'Rondônia', 'RO', 1),
(22, 'Roraima', 'RR', 1),
(23, 'Santa Catarina', 'SC', 1),
(24, 'São Paulo', 'SP', 1),
(25, 'Sergipe', 'SE', 1),
(26, 'Tocantins', 'TO', 1),
(27, 'Distrito Federal', 'DF', 1);

--
-- Índices para tabelas despejadas
--

--
-- Índices para tabela `Estados`
--
ALTER TABLE `Estados`
  ADD PRIMARY KEY (`EstadoId`);

--
-- AUTO_INCREMENT de tabelas despejadas
--

--
-- AUTO_INCREMENT de tabela `Estados`
--
ALTER TABLE `Estados`
  MODIFY `EstadoId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=28;
COMMIT;