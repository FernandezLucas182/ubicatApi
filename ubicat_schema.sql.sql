-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 03-02-2026 a las 00:11:55
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `ubicat`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `categorias`
--

CREATE TABLE `categorias` (
  `idCategoria` int(11) NOT NULL,
  `nombreCategoria` varchar(50) NOT NULL,
  `descripcion` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- Volcado de datos para la tabla `categorias`
--

INSERT INTO `categorias` (`idCategoria`, `nombreCategoria`, `descripcion`) VALUES
(1, 'Salud', 'Consejos de bienestar animal y salud veterinaria'),
(2, 'Cuidados', 'Cuidado diario de mascotas'),
(3, 'Denuncias', 'Reportes de maltrato, abandono o abuso'),
(4, 'Encontrados', 'Mascotas encontradas'),
(5, 'Perdidos', 'Mascotas perdidas');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `comentarios`
--

CREATE TABLE `comentarios` (
  `idComentario` int(11) NOT NULL,
  `idUsuario` int(11) NOT NULL,
  `idForo` int(11) NOT NULL,
  `fecha` datetime DEFAULT current_timestamp(),
  `texto` text NOT NULL,
  `validado` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- Volcado de datos para la tabla `comentarios`
--

INSERT INTO `comentarios` (`idComentario`, `idUsuario`, `idForo`, `fecha`, `texto`, `validado`) VALUES
(1, 4, 1, '2025-11-18 18:52:02', 'La vi cerca del supermercado ayer.', 0),
(2, 3, 1, '2025-11-18 18:52:02', 'Recomiendo revisar cámaras de la zona.', 1),
(3, 1, 3, '2025-11-18 18:52:02', 'Muy buena información, gracias!', 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `foro`
--

CREATE TABLE `foro` (
  `idForo` int(11) NOT NULL,
  `idUsuario` int(11) NOT NULL,
  `idCategoria` int(11) NOT NULL,
  `titulo` varchar(100) NOT NULL,
  `descripcion` text DEFAULT NULL,
  `imagenPrincipal` varchar(255) DEFAULT NULL,
  `fechaPublicacion` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- Volcado de datos para la tabla `foro`
--

INSERT INTO `foro` (`idForo`, `idUsuario`, `idCategoria`, `titulo`, `descripcion`, `imagenPrincipal`, `fechaPublicacion`) VALUES
(1, 1, 5, 'Perdí a mi gata Michi', 'Se perdió anoche en la zona de Juana Koslay', 'michi-perdida.jpg', '2025-11-18 18:51:13'),
(2, 2, 4, 'Perro encontrado en el centro', 'Encontré un perrito adulto cerca de la plaza', 'toby-encontrado.jpg', '2025-11-18 18:51:13'),
(3, 3, 1, 'Consejo: Desparasitación', 'Información básica sobre parasitosis', 'desparasitar.jpg', '2025-11-18 18:51:13');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `fotosforo`
--

CREATE TABLE `fotosforo` (
  `idFotos` int(11) NOT NULL,
  `idForo` int(11) NOT NULL,
  `idUsuario` int(11) NOT NULL,
  `urlFoto` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- Volcado de datos para la tabla `fotosforo`
--

INSERT INTO `fotosforo` (`idFotos`, `idForo`, `idUsuario`, `urlFoto`) VALUES
(1, 1, 1, 'foto1-michi.jpg'),
(2, 1, 1, 'foto2-michi.jpg'),
(3, 2, 2, 'toby-foto1.jpg'),
(4, 3, 3, 'consejo-salud1.jpg');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `mascota`
--

CREATE TABLE `mascota` (
  `idMascota` int(11) NOT NULL,
  `idUsuario` int(11) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `especie` varchar(50) NOT NULL,
  `edad` int(11) DEFAULT NULL,
  `enfermedades` text DEFAULT NULL,
  `cuidados` text DEFAULT NULL,
  `estado` enum('perdida','encontrada','adoptada','en casa') DEFAULT 'en casa',
  `foto` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- Volcado de datos para la tabla `mascota`
--

INSERT INTO `mascota` (`idMascota`, `idUsuario`, `nombre`, `especie`, `edad`, `enfermedades`, `cuidados`, `estado`, `foto`) VALUES
(1, 1, 'Firulais', 'Perro', 4, 'Sin antecedentes', 'Baño mensual, comida balanceada', 'en casa', 'mascota_default.png'),
(2, 1, 'Michi', 'Gato', 2, 'Alergia leve', 'Evitar polvo y humedad', 'perdida', 'michi.png'),
(3, 2, 'Toby', 'Perro', 6, 'Artritis', 'Caminatas cortas, medicación', 'encontrada', 'toby.jpg'),
(4, 5, 'Milu', 'Perro', 4, 'Ninguna', 'Comer 2 veces al día', 'perdida', 'milu.png'),
(5, 5, 'Milu', 'Perro', 4, 'Ninguna', 'Comer 2 veces al día', 'perdida', 'milu.png'),
(6, 5, 'Rocky', 'Perro', 10, 'Diabetes', 'Comer 2 veces al día', 'en casa', 'milu.png'),
(7, 5, 'Milu', 'Perro', 4, 'Ninguna', 'Comer 2 veces al día', 'perdida', 'milu.png'),
(8, 5, 'Milu', 'Perro', 4, 'Ninguna', 'Comer 2 veces al día', 'perdida', 'milu.png'),
(9, 5, 'Ares', 'Gato', 9, 'Ninguna', 'Comer 6 veces al día, croquetas y alimento humedo', 'en casa', 'milu.png'),
(10, 5, 'Cirus', 'Gato', 3, 'Ninguna', 'Comer 6 veces al día, croquetas y alimento humedo', 'en casa', 'milu.png'),
(11, 5, 'Lorenzo', 'Loro', 8, 'Ninguna', 'Comer 4 veces al dia, frutas', 'en casa', 'milu.png');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `mascotareportefoto`
--

CREATE TABLE `mascotareportefoto` (
  `id` int(11) NOT NULL,
  `idMascota` int(11) NOT NULL,
  `nombreArchivo` varchar(255) NOT NULL,
  `fecha` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qr`
--

CREATE TABLE `qr` (
  `idQR` int(11) NOT NULL,
  `idUsuario` int(11) NOT NULL,
  `idMascota` int(11) NOT NULL,
  `codigo` varchar(255) NOT NULL,
  `urlDatosMascota` varchar(255) NOT NULL,
  `fechaGeneracion` datetime DEFAULT current_timestamp(),
  `ubicacionActual` varchar(255) DEFAULT NULL,
  `fechaUltimoEscaneo` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- Volcado de datos para la tabla `qr`
--

INSERT INTO `qr` (`idQR`, `idUsuario`, `idMascota`, `codigo`, `urlDatosMascota`, `fechaGeneracion`, `ubicacionActual`, `fechaUltimoEscaneo`) VALUES
(1, 1, 1, 'QR001-FIRULAIS', 'www.ubicat.com/mascota/1', '2025-11-18 18:48:19', 'San Luis, Centro', NULL),
(2, 1, 2, 'QR002-MICHI', 'www.ubicat.com/mascota/2', '2025-11-18 18:48:19', 'San Luis, Juana Koslay', NULL),
(3, 2, 3, 'QR003-TOBY', 'www.ubicat.com/mascota/3', '2025-11-18 18:48:19', 'San Luis, UNSL', NULL),
(4, 5, 4, '1f833a21', 'https://ubicat.com/mascota/4', '2025-11-21 21:22:53', 'Av San Martín 1234, Posadas', '2026-01-07 05:10:31.502297'),
(5, 5, 5, 'c1d86b53', 'https://ubicat.com/mascota/5', '2025-11-21 21:45:42', 'Sin ubicación', NULL),
(6, 5, 6, 'b8cff387', 'https://ubicat.com/mascota/6', '2025-11-24 19:07:15', 'Sin ubicación', NULL),
(7, 5, 7, '7962969e', 'https://ubicat.com/mascota/7', '2025-11-24 19:08:27', 'Sin ubicación', NULL),
(8, 5, 8, 'fb9f25f9', 'https://ubicat.com/mascota/8', '2025-11-24 19:08:33', 'Sin ubicación', NULL),
(9, 5, 9, '827cdb2b', 'https://ubicat.com/mascota/9', '2025-11-24 19:31:26', 'Sin ubicación', NULL),
(10, 5, 10, '4fbe42a4', 'https://ubicat.com/mascota/10', '2025-11-24 19:36:55', 'Sin ubicación', NULL),
(11, 5, 11, 'f4070a49', 'https://ubicat.com/mascota/11', '2025-11-24 19:42:57', 'Sin ubicación', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `reporte_mascota`
--

CREATE TABLE `reporte_mascota` (
  `idReporte` int(11) NOT NULL,
  `idMascota` int(11) NOT NULL,
  `idUsuarioReporta` int(11) DEFAULT NULL,
  `tipoReporte` varchar(50) NOT NULL,
  `ubicacion` text DEFAULT NULL,
  `mensaje` text DEFAULT NULL,
  `fecha` datetime NOT NULL,
  `foto` varchar(255) DEFAULT NULL,
  `emailReportante` varchar(120) DEFAULT NULL,
  `telefonoReportante` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- Volcado de datos para la tabla `reporte_mascota`
--

INSERT INTO `reporte_mascota` (`idReporte`, `idMascota`, `idUsuarioReporta`, `tipoReporte`, `ubicacion`, `mensaje`, `fecha`, `foto`, `emailReportante`, `telefonoReportante`) VALUES
(1, 4, 5, 'perdida', NULL, NULL, '2025-12-10 09:23:45', NULL, NULL, NULL),
(2, 4, 5, 'perdida', NULL, NULL, '2025-12-11 09:32:19', NULL, NULL, NULL),
(3, 5, NULL, 'vista', 'plaza central', 'la vi cerca del kiosco', '2026-01-06 20:44:28', '/uploads/reportes/2026/01/f991dfe0d3ac49abb34958bf5a65698b.jpg', NULL, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario`
--

CREATE TABLE `usuario` (
  `idUsuario` int(11) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `apellido` varchar(50) DEFAULT NULL,
  `telefono` varchar(30) DEFAULT NULL,
  `email` varchar(100) NOT NULL,
  `ubicacion` varchar(100) DEFAULT NULL,
  `rol` enum('dueño','veterinario','rescatista','publico') NOT NULL,
  `especialidad` varchar(100) DEFAULT NULL,
  `password` varchar(255) NOT NULL,
  `clasificacionPromedio` decimal(3,2) DEFAULT 0.00
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- Volcado de datos para la tabla `usuario`
--

INSERT INTO `usuario` (`idUsuario`, `nombre`, `apellido`, `telefono`, `email`, `ubicacion`, `rol`, `especialidad`, `password`, `clasificacionPromedio`) VALUES
(1, 'Lucas', 'Fernandez', '2664123456', 'lucas@gmail.com', 'San Luis', 'dueño', NULL, '', 0.00),
(2, 'Ana', 'Martinez', '2664789000', 'ana.rescatista@gmail.com', 'San Luis', 'rescatista', NULL, '', 0.00),
(3, 'Dr. Pablo', 'Suarez', '2664567890', 'pablo.vet@gmail.com', 'San Luis', 'veterinario', 'Medicina Felina y Canina', '', 4.50),
(4, 'Maria', 'Gomez', '2664010203', 'maria@gmail.com', 'San Luis', 'publico', NULL, '', 0.00),
(5, 'Lucas', 'Fernandez', '2664000000', 'lucas@example.com', 'San Luis', '', NULL, 'XT3eYcd5TfmjiYPH+qTcz3emzArynPjlisv8jKCCeSM=', 0.00);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `validaciones`
--

CREATE TABLE `validaciones` (
  `idValidacion` int(11) NOT NULL,
  `idComentario` int(11) NOT NULL,
  `idUsuarioValidador` int(11) NOT NULL,
  `observacion` text DEFAULT NULL,
  `puntuacion` int(11) DEFAULT NULL CHECK (`puntuacion` between 0 and 5)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- Volcado de datos para la tabla `validaciones`
--

INSERT INTO `validaciones` (`idValidacion`, `idComentario`, `idUsuarioValidador`, `observacion`, `puntuacion`) VALUES
(1, 2, 3, 'Revisión médica confirmada. Información correcta.', 5);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20260105234451_Inicial', '9.0.0'),
('20260107023257_AddFotoToReporteMascota', '9.0.0'),
('20260107102004_RelacionMascotaUsuario', '9.0.0'),
('20260107102417_MascotaUsuarioLink', '9.0.0'),
('20260107105230_FechaUltimoEscaneoQR', '9.0.0'),
('BASELINE_EXISTENTE', '9.0.0');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `categorias`
--
ALTER TABLE `categorias`
  ADD PRIMARY KEY (`idCategoria`);

--
-- Indices de la tabla `comentarios`
--
ALTER TABLE `comentarios`
  ADD PRIMARY KEY (`idComentario`),
  ADD KEY `idUsuario` (`idUsuario`),
  ADD KEY `idForo` (`idForo`);

--
-- Indices de la tabla `foro`
--
ALTER TABLE `foro`
  ADD PRIMARY KEY (`idForo`),
  ADD KEY `idUsuario` (`idUsuario`),
  ADD KEY `idCategoria` (`idCategoria`);

--
-- Indices de la tabla `fotosforo`
--
ALTER TABLE `fotosforo`
  ADD PRIMARY KEY (`idFotos`),
  ADD KEY `idForo` (`idForo`),
  ADD KEY `idUsuario` (`idUsuario`);

--
-- Indices de la tabla `mascota`
--
ALTER TABLE `mascota`
  ADD PRIMARY KEY (`idMascota`),
  ADD KEY `IX_mascota_idUsuario` (`idUsuario`);

--
-- Indices de la tabla `mascotareportefoto`
--
ALTER TABLE `mascotareportefoto`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idMascota` (`idMascota`);

--
-- Indices de la tabla `qr`
--
ALTER TABLE `qr`
  ADD PRIMARY KEY (`idQR`),
  ADD UNIQUE KEY `idMascota` (`idMascota`),
  ADD UNIQUE KEY `codigo` (`codigo`),
  ADD KEY `idUsuario` (`idUsuario`);

--
-- Indices de la tabla `reporte_mascota`
--
ALTER TABLE `reporte_mascota`
  ADD PRIMARY KEY (`idReporte`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`idUsuario`),
  ADD UNIQUE KEY `email` (`email`);

--
-- Indices de la tabla `validaciones`
--
ALTER TABLE `validaciones`
  ADD PRIMARY KEY (`idValidacion`),
  ADD KEY `idComentario` (`idComentario`),
  ADD KEY `idUsuarioValidador` (`idUsuarioValidador`);

--
-- Indices de la tabla `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `categorias`
--
ALTER TABLE `categorias`
  MODIFY `idCategoria` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `comentarios`
--
ALTER TABLE `comentarios`
  MODIFY `idComentario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `foro`
--
ALTER TABLE `foro`
  MODIFY `idForo` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `fotosforo`
--
ALTER TABLE `fotosforo`
  MODIFY `idFotos` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `mascota`
--
ALTER TABLE `mascota`
  MODIFY `idMascota` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT de la tabla `mascotareportefoto`
--
ALTER TABLE `mascotareportefoto`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qr`
--
ALTER TABLE `qr`
  MODIFY `idQR` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT de la tabla `reporte_mascota`
--
ALTER TABLE `reporte_mascota`
  MODIFY `idReporte` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `idUsuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `validaciones`
--
ALTER TABLE `validaciones`
  MODIFY `idValidacion` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `comentarios`
--
ALTER TABLE `comentarios`
  ADD CONSTRAINT `comentarios_ibfk_1` FOREIGN KEY (`idUsuario`) REFERENCES `usuario` (`idUsuario`) ON DELETE CASCADE,
  ADD CONSTRAINT `comentarios_ibfk_2` FOREIGN KEY (`idForo`) REFERENCES `foro` (`idForo`) ON DELETE CASCADE;

--
-- Filtros para la tabla `foro`
--
ALTER TABLE `foro`
  ADD CONSTRAINT `foro_ibfk_1` FOREIGN KEY (`idUsuario`) REFERENCES `usuario` (`idUsuario`) ON DELETE CASCADE,
  ADD CONSTRAINT `foro_ibfk_2` FOREIGN KEY (`idCategoria`) REFERENCES `categorias` (`idCategoria`) ON DELETE CASCADE;

--
-- Filtros para la tabla `fotosforo`
--
ALTER TABLE `fotosforo`
  ADD CONSTRAINT `fotosforo_ibfk_1` FOREIGN KEY (`idForo`) REFERENCES `foro` (`idForo`) ON DELETE CASCADE,
  ADD CONSTRAINT `fotosforo_ibfk_2` FOREIGN KEY (`idUsuario`) REFERENCES `usuario` (`idUsuario`) ON DELETE CASCADE;

--
-- Filtros para la tabla `mascota`
--
ALTER TABLE `mascota`
  ADD CONSTRAINT `FK_mascota_usuario_idUsuario` FOREIGN KEY (`idUsuario`) REFERENCES `usuario` (`idUsuario`) ON DELETE CASCADE,
  ADD CONSTRAINT `mascota_ibfk_1` FOREIGN KEY (`idUsuario`) REFERENCES `usuario` (`idUsuario`) ON DELETE CASCADE;

--
-- Filtros para la tabla `mascotareportefoto`
--
ALTER TABLE `mascotareportefoto`
  ADD CONSTRAINT `mascotareportefoto_ibfk_1` FOREIGN KEY (`idMascota`) REFERENCES `mascota` (`idMascota`);

--
-- Filtros para la tabla `qr`
--
ALTER TABLE `qr`
  ADD CONSTRAINT `qr_ibfk_1` FOREIGN KEY (`idUsuario`) REFERENCES `usuario` (`idUsuario`) ON DELETE CASCADE,
  ADD CONSTRAINT `qr_ibfk_2` FOREIGN KEY (`idMascota`) REFERENCES `mascota` (`idMascota`) ON DELETE CASCADE;

--
-- Filtros para la tabla `validaciones`
--
ALTER TABLE `validaciones`
  ADD CONSTRAINT `validaciones_ibfk_1` FOREIGN KEY (`idComentario`) REFERENCES `comentarios` (`idComentario`) ON DELETE CASCADE,
  ADD CONSTRAINT `validaciones_ibfk_2` FOREIGN KEY (`idUsuarioValidador`) REFERENCES `usuario` (`idUsuario`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
