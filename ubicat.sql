-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 28-10-2026 a las 19:18:05
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
  `foto` varchar(255) DEFAULT NULL,
  `fechaPerdida` datetime(6) DEFAULT NULL,
  `latitudPerdida` double DEFAULT NULL,
  `longitudPerdida` double DEFAULT NULL,
  `radioBusquedaMetros` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- Volcado de datos para la tabla `mascota`
--

INSERT INTO `mascota` (`idMascota`, `idUsuario`, `nombre`, `especie`, `edad`, `enfermedades`, `cuidados`, `estado`, `foto`, `fechaPerdida`, `latitudPerdida`, `longitudPerdida`, `radioBusquedaMetros`) VALUES
(1, 1, 'Firulais', 'Perro', 4, 'Sin antecedentes', 'Baño mensual, comida balanceada', 'en casa', 'mascota_default.png', NULL, NULL, NULL, 0),
(2, 1, 'Michi', 'Gato', 2, 'Alergia leve', 'Evitar polvo y humedad', 'perdida', 'michi.png', NULL, -33.2898, -66.3265, 500),
(3, 2, 'Toby', 'Perro', 6, 'Artritis', 'Caminatas cortas, medicación', 'encontrada', 'toby.jpg', NULL, NULL, NULL, 0),
(4, 5, 'Milu', 'Perro', 4, 'Ninguna', 'Comer 2 veces al día', 'perdida', 'milu.png', '2026-10-27 23:25:24.883897', -33.29249164421812, -66.25577705275784, 2000),
(5, 5, 'Milu antigua 5', 'Perro', 4, 'Ninguna', 'Comer 2 veces al día', 'perdida', 'milu.png', NULL, -33.3017, -66.3378, 500),
(6, 5, 'Rocky', 'Perro', 10, 'Diabetes', 'Comer 2 veces al día', 'perdida', 'milu.png', NULL, -33.2945, -66.3505, 500),
(7, 5, 'Milu antigua 7', 'Perro', 4, 'Ninguna', 'Comer 2 veces al día', 'perdida', 'milu.png', NULL, -33.3085, -66.3225, 500),
(8, 5, 'Milu antigua 8', 'Perro', 4, 'Ninguna', 'Comer 2 veces al día', 'perdida', 'milu.png', NULL, -33.2889, -66.2666, 500),
(9, 5, 'Ares', 'Gato', 9, 'Ninguna', 'Comer 6 veces al día, croquetas y alimento humedo', 'en casa', 'bc6a081a-9df7-4ba2-8d67-8fae806b1bdd.jpg', NULL, NULL, NULL, 0),
(10, 5, 'Cirus', 'Gato', 3, 'Ninguna', 'Comer 6 veces al día, croquetas y alimento humedo', 'en casa', '54720254-e374-477f-bd30-e0b62aff8c2f.jpg', NULL, NULL, NULL, 0),
(11, 5, 'Lorenzo', 'Loro', 8, 'Ninguna', 'Comer 4 veces al dia, frutas', 'perdida', 'milu.png', '2026-10-26 21:35:30.879087', -33.2889, -66.2666, 500),
(12, 5, 'Doris', 'Perra', 12, 'diabetes', 'Requiere medicacion diaria para la diabetes', '', 'https://example.com/foto.jpg', NULL, NULL, NULL, 0),
(13, 5, 'Yalca', 'zorro', 2, 'no se sabe', 'Requiere medicacion diaria para la diabetes', '', 'https://example.com/foto.jpg', NULL, NULL, NULL, 0),
(14, 5, 'Roberta', 'Rata', 1, 'no', 'le gustan las semillas o verduras, las harinas le hacen mal', '', 'https://example.com/foto.jpg', NULL, NULL, NULL, 0),
(15, 6, 'Jacinta', 'Rata', 3, 'no', 'le gustan las semillas o verduras, las harinas le hacen mal', 'en casa', 'cec36e09-7167-48a7-8ac6-57a0946acf03.jpg', NULL, NULL, NULL, 0),
(16, 6, 'Lorena', 'Lora', 5, 'no', 'nada de carne', 'perdida', '3f06b1c6-c32e-47bd-96aa-c2cad69f6dff.jpg', NULL, -33.325, -66.329, 500),
(17, 6, 'ciruel', 'gato', 3, 'no', 'urinarios. alimento urinary si o si', 'en casa', 'bb7cd9e1-a23c-4a3b-afeb-4049407f3da1.jpg', NULL, NULL, NULL, 0),
(18, 6, 'zorak', 'Lagarto', 5, 'no', 'varias horas de sol', 'en casa', '6358b211-f003-47b8-81eb-6438b585fd76.jpg', NULL, NULL, NULL, 0),
(19, 6, 'Hissu', 'mono', 5, 'ninguna', 'paseos y dieta balanceada', '', '4b90bb35-b3e6-410a-9777-ba3aa44c7ee4.jpg', NULL, NULL, NULL, 0),
(20, 6, 'Rabi', 'conejo', 2, 'ninguna', 'frutas, verduras, y sol', '', '3b380722-d727-4fc7-9ce9-f8f455466bf7.jpg', NULL, NULL, NULL, 0),
(21, 5, 'mona', 'gato tricolor', 1, 'no', 'comida balanceada y casera', 'en casa', '95cfbd25-db8c-4a9a-bdb1-2274f7435c0b.jpg', NULL, NULL, NULL, 0),
(22, 5, 'cerafino', 'gato', 1, 'no', 'croquetas, pollo, verduras varias', 'en casa', 'fe3959db-09bf-484c-b497-0fff103d0886.jpg', NULL, NULL, NULL, 0),
(23, 7, 'Rómulo', 'doberman', 3, 'no', 'normal', 'en casa', 'b227478b-44ec-444f-a51c-e926213c69e0.jpg', NULL, NULL, NULL, 0),
(25, 10, 'romu', 'doberman', 3, 'no', 'paseos y alimento balanciado', 'en casa', 'b2c45e84-7409-4cc8-8f5b-8b732e82f1a9.jpg', NULL, NULL, NULL, 0),
(26, 5, 'firulai', 'perro', 2, 'no', 'no', 'en casa', 'fdbc20d9-429d-44af-88be-6775f0420f4e.jpg', NULL, NULL, NULL, 0),
(27, 12, 'cus cus', 'raton', 2, 'ninguna', 'frutas, verduras, semillas, temperatura templada', 'en casa', 'ceeca5bf-5365-4fb4-abe5-2259a039078e.jpg', NULL, -33.3017, -66.3378, 1000);

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
  `fechaUltimoEscaneo` datetime(6) DEFAULT NULL,
  `latitudActual` double DEFAULT NULL,
  `longitudActual` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- Volcado de datos para la tabla `qr`
--

INSERT INTO `qr` (`idQR`, `idUsuario`, `idMascota`, `codigo`, `urlDatosMascota`, `fechaGeneracion`, `ubicacionActual`, `fechaUltimoEscaneo`, `latitudActual`, `longitudActual`) VALUES
(1, 1, 1, 'QR001-FIRULAIS', 'www.ubicat.com/mascota/1', '2025-11-18 18:48:19', 'San Luis, Centro', NULL, -33.3017, -66.3378),
(2, 1, 2, 'QR002-MICHI', 'www.ubicat.com/mascota/2', '2025-11-18 18:48:19', 'San Luis, Juana Koslay', NULL, -33.2898, -66.3265),
(3, 2, 3, 'QR003-TOBY', 'www.ubicat.com/mascota/3', '2025-11-18 18:48:19', 'plaza pringles', '2026-10-11 19:49:48.107065', -33.3017, -66.3378),
(4, 5, 4, '1f833a21', 'https://ubicat.com/mascota/4', '2025-11-21 21:22:53', '20 de Noviembre 1229, D5701 San Luis, Argentina', '2026-10-26 19:18:02.032364', -33.291455, -66.2670628),
(5, 5, 5, 'c1d86b53', 'https://ubicat.com/mascota/5', '2025-11-21 21:45:42', 'plaza independecia', '2026-10-24 17:16:32.952361', -33.3017, -66.3378),
(6, 5, 6, 'b8cff387', 'https://ubicat.com/mascota/6', '2025-11-24 19:07:15', 'Zona de prueba - San Luis', NULL, -33.2945, -66.3505),
(7, 5, 7, '7962969e', 'https://ubicat.com/mascota/7', '2025-11-24 19:08:27', 'Zona de prueba - San Luis', NULL, -33.3085, -66.3225),
(8, 5, 8, 'fb9f25f9', 'https://ubicat.com/mascota/8', '2025-11-24 19:08:33', 'Juana koslay', NULL, -33.2889, -66.2666),
(9, 5, 9, '827cdb2b', 'https://ubicat.com/mascota/9', '2025-11-24 19:31:26', 'Juana koslay', NULL, -33.2889, -66.2666),
(10, 5, 10, '4fbe42a4', 'https://ubicat.com/mascota/10', '2025-11-24 19:36:55', 'Juana koslay', NULL, -33.2889, -66.2666),
(11, 5, 11, 'f4070a49', 'https://ubicat.com/mascota/11', '2025-11-24 19:42:57', 'Juana koslay', NULL, -33.2889, -66.2666),
(12, 5, 12, '9241d00e', 'https://ubicat.com/mascota/12', '2026-09-22 21:02:01', 'Juana koslay', NULL, -33.2889, -66.2666),
(13, 5, 13, 'c9d3c500', 'https://ubicat.com/mascota/13', '2026-09-23 01:08:08', 'Juana koslay', NULL, -33.2889, -66.2666),
(14, 5, 14, 'ced20634', 'https://ubicat.com/mascota/14', '2026-09-23 01:12:53', 'Juana koslay', NULL, -33.2889, -66.2666),
(15, 6, 15, '66f0cc6c', 'https://ubicat.com/mascota/15', '2026-09-24 16:53:22', 'Juana Joslay', NULL, -33.2889, -66.2666),
(16, 6, 16, 'c1d8402f', 'https://ubicat.com/mascota/16', '2026-09-25 16:57:23', 'Zona de prueba - San Luis', '2026-09-25 18:05:56.273805', -33.325, -66.329),
(17, 6, 18, 'b9bc652e', 'https://ubicat.com/mascota/18', '2026-10-07 08:45:06', 'Juana Joslay', NULL, -33.2889, -66.2666),
(18, 6, 19, 'cbe3b055', 'https://ubicat.com/mascota/19', '2026-10-07 08:55:30', 'Juana Joslay', NULL, -33.2889, -66.2666),
(19, 6, 20, '68451ce3', 'https://ubicat.com/mascota/20', '2026-10-07 09:26:06', 'Juana Joslay', NULL, -33.2889, -66.2666),
(20, 5, 21, 'e1a5e7cd', 'https://ubicat.com/mascota/21', '2026-10-07 10:03:43', 'Juana koslay', NULL, -33.2889, -66.2666),
(21, 5, 22, '41cf4124', 'https://ubicat.com/mascota/22', '2026-10-07 11:06:43', 'Juana koslay', NULL, -33.2889, -66.2666),
(22, 7, 23, 'da997a4a', 'https://ubicat.com/mascota/23', '2026-10-12 15:18:04', 'Barrio Félix Bogado Manzana o Casa 5', NULL, -33.3017, -66.3378),
(24, 10, 25, '20477fea', 'https://ubicat.com/mascota/25', '2026-10-12 16:44:02', 'Tomas Cuello Y Angel Domínguez', NULL, -33.3017, -66.3378),
(25, 5, 26, 'c22e360c', 'https://ubicat.com/mascota/26', '2026-10-17 22:03:56', 'Juana koslay', NULL, -33.2889, -66.2666),
(26, 12, 27, '195b00eb', 'https://ubicat.com/mascota/27', '2026-10-26 01:10:09', 'San Luis', NULL, -33.3017, -66.3378);

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
  `telefonoReportante` varchar(50) DEFAULT NULL,
  `latitud` double DEFAULT NULL,
  `longitud` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- Volcado de datos para la tabla `reporte_mascota`
--

INSERT INTO `reporte_mascota` (`idReporte`, `idMascota`, `idUsuarioReporta`, `tipoReporte`, `ubicacion`, `mensaje`, `fecha`, `foto`, `emailReportante`, `telefonoReportante`, `latitud`, `longitud`) VALUES
(1, 4, 5, 'perdida', NULL, NULL, '2025-12-10 09:23:45', NULL, NULL, NULL, NULL, NULL),
(2, 4, 5, 'perdida', NULL, NULL, '2025-12-11 09:32:19', NULL, NULL, NULL, NULL, NULL),
(3, 5, NULL, 'vista', 'plaza central', 'la vi cerca del kiosco', '2026-01-06 20:44:28', '/uploads/reportes/2026/01/f991dfe0d3ac49abb34958bf5a65698b.jpg', NULL, NULL, NULL, NULL),
(4, 16, NULL, 'vista', 'Ex estacion de trenes', 'por las vias', '2026-09-25 17:58:27', '/uploads/reportes/2026/09/99bec646ee46474098bd527c4da7f31e.jpg', NULL, NULL, NULL, NULL),
(5, 3, NULL, 'vista', 'colon y belgrano', 'se encuentra echado en la esquina', '2026-10-11 18:42:18', '/reportes/46587348-e49f-4e26-af2d-f0dcbc645ca6.jfif', NULL, NULL, NULL, NULL),
(6, 3, NULL, 'vista', 'ahora esta afuera del aiello del centro', 'esta desorientado', '2026-10-11 19:06:39', '/reportes/6477d2fc-4918-4608-b35d-9c7d291608ef.jpg', NULL, NULL, NULL, NULL),
(7, 3, NULL, 'vista', 'colon y Junin', 'esta echado fuera de la carniceria', '2026-10-11 19:11:38', '/reportes/7c64fb7c-bcf9-47e7-9213-9e3f949c99c7.jpg', 'fernandez19lucas13@gmail.com', '2665250181', NULL, NULL),
(8, 3, NULL, 'vista', 'por la calle 20 de noviembre', 'corria para la plaza 20 de noviembre', '2026-10-11 19:38:54', '/reportes/02945a8d-224a-422d-a8dc-441b43a82f0d.jpg', 'fernandez19lucas13@gmail.com', '2665250181', NULL, NULL),
(9, 3, NULL, 'vista', 'plaza pringles', 'esta hambriento', '2026-10-11 19:49:48', '/reportes/1ddd8d3f-a6b6-45f7-9d94-5ced328ec9bc.jpg', 'fernandez19lucas13@gmail.com', '2665250181', NULL, NULL),
(10, 4, NULL, 'vista', '20 de Noviembre 1229, D5701 San Luis, Argentina', 'Corriendo por la 20 d noviembre hacia Av. Viento chorrillero', '2026-10-11 20:42:53', '/reportes/3ec1b9dc-fd5f-4563-b5d5-82763a0ea001.jpg', 'fernandez19lucas13@gmail.com', '2665250181', NULL, NULL),
(11, 4, NULL, 'vista', '20 de Noviembre 1229, D5701 San Luis, Argentina', 'Corriendo por la 20 d noviembre hacia Av. Viento chorrillero', '2026-10-11 20:43:29', '/reportes/53e46b1d-ad93-439d-80e9-7ff4274371cd.jpg', 'fernandez19lucas13@gmail.com', '2665250181', NULL, NULL),
(12, 5, NULL, 'vista', 'plaza independecia', 'dormida en un banco', '2026-10-24 17:16:32', NULL, 'prueba222@ejemplo.com', '2664000000', -33.3017, -66.3378),
(13, 4, NULL, 'vista', '20 de Noviembre 1229, D5701 San Luis, Argentina', 'La Vi fuera de casa', '2026-10-26 01:41:07', NULL, 'fernandez19lucas13@gmail.com', '2665250181', NULL, NULL),
(14, 4, NULL, 'vista', '20 de Noviembre 1229, D5701 San Luis, Argentina', 'La tengo yo', '2026-10-26 18:20:44', NULL, 'fernandez19lucas88@gmail.com', '26665287', -33.291455, -66.2670628),
(15, 4, 5, 'perdida', NULL, 'Bla bla', '2026-10-27 23:19:57', NULL, 'fernandez19lucas13@gmail.com', '2665250181', NULL, NULL),
(16, 4, 5, 'perdida', NULL, 'Ja', '2026-10-27 23:25:24', NULL, 'fernandez19lucas13@gmail.com', '2665250181', NULL, NULL);

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
  `rol` enum('dueño','veterinario','rescatista','admin') NOT NULL,
  `especialidad` varchar(100) DEFAULT NULL,
  `password` varchar(255) NOT NULL,
  `clasificacionPromedio` decimal(3,2) DEFAULT 0.00,
  `fotoPerfil` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- Volcado de datos para la tabla `usuario`
--

INSERT INTO `usuario` (`idUsuario`, `nombre`, `apellido`, `telefono`, `email`, `ubicacion`, `rol`, `especialidad`, `password`, `clasificacionPromedio`, `fotoPerfil`) VALUES
(1, 'Lucas', 'Fernandez', '2664123456', 'lucas@gmail.com', 'San Luis', 'dueño', NULL, '', 0.00, NULL),
(2, 'Oscar', 'Ferandez', '2664850741', 'of0053644@gmail.com', 'San Luis', 'rescatista', NULL, '', 0.00, NULL),
(3, 'Dr. Pablo', 'Suarez', '2664567890', 'pablo.vet@gmail.com', 'San Luis', 'veterinario', 'Medicina Felina y Canina', '', 4.50, NULL),
(4, 'Maria', 'Gomez', '2664010203', 'maria@gmail.com', 'San Luis', 'veterinario', NULL, '', 0.00, NULL),
(5, 'Lucas max', 'Fernandez', '266400011', 'lucas@example.com', 'Juana koslay', 'dueño', NULL, 'XT3eYcd5TfmjiYPH+qTcz3emzArynPjlisv8jKCCeSM=', 0.00, '/uploads/perfiles/0f78624f58f34d848c1e9a5ff357e049.jpg'),
(6, 'Juan Gabriel', 'Sarazena', '2664889966', 'Sarazena22@example.com', 'Juana Joslay', 'dueño', NULL, 'XT3eYcd5TfmjiYPH+qTcz3emzArynPjlisv8jKCCeSM=', 0.00, NULL),
(7, 'Valentin', 'Fernandez', '2665008290', 'valenfernandez913@gmail.com', 'Barrio Félix Bogado Manzana o Casa 5', 'dueño', NULL, 'ffb7ODTFPiAOWD8N6pct+QpQhsMczWlmtOBrZoY3jKM=', 0.00, '/uploads/perfiles/ad3c2beb9e704f988571739630d6a912.jpg'),
(10, 'Flor', 'Fernández', '2664963559', 'fflorfernandez7@gmail.com', 'Tomas Cuello Y Angel Domínguez', 'dueño', NULL, 'QwT7xhgFkgTvM9OTDynfyLeMgQB7iOPkZKPPrja8I5A=', 0.00, '/uploads/perfiles/58a8d1ef7ee04549864e044d964c82e9.jpg'),
(11, 'jose', 'lucero', '2664123456', 'joseLucero@example.com', 'bolivar y chacabuco', 'veterinario', NULL, 'ODoP9KXNWVBdfSN8CfyDgNQznyb2NwIxbVrdkHPRRuo=', 0.00, '/uploads/perfiles/fcb1f05145fd485f87b3f199516f51f8.jpg'),
(12, 'Admin', 'Ubicat', '2664000000', 'admin@ubicat.local', 'San Luis', 'admin', NULL, 'aETh2gGyQj6sU87UQvVND5TyHL1RrQt2B0zsr6PPtIE=', 0.00, '/uploads/perfiles/e062bbddbce5431f9b373fefaf8445df.png'),
(13, 'martina', 'Lopez', '2665789453', 'martina.lopez@ubicat.test', 'jk', 'veterinario', NULL, 'OWvc8VkzUKFLZAlDhyIDW6lzRg2ksYxBdyqtVpmmMEA=', 0.00, '/uploads/perfiles/351d6060da264be6b1208041298288c3.jpg'),
(14, 'Nicolas', 'Torres', '2664741852', 'nicolas.torres@ubicat.test', 'caseros 342', 'veterinario', NULL, 'ODoP9KXNWVBdfSN8CfyDgNQznyb2NwIxbVrdkHPRRuo=', 0.00, '/uploads/perfiles/c7e9fe3a3e734ab68f9c5fad3c800a71.jpg'),
(15, 'Camila', 'Fernandez', '2664741852', 'camila.fernandez@ubicat.test', 'buenos aires 212', 'veterinario', NULL, 'ODoP9KXNWVBdfSN8CfyDgNQznyb2NwIxbVrdkHPRRuo=', 0.00, '/uploads/perfiles/0d688f6d16e64740bfcc31fcb9dc94b8.jpg'),
(16, 'Diego', 'Ramirez', '2664568974', 'dieg.ramirez@ubicat.test', 'mitre 234', 'veterinario', NULL, 'ODoP9KXNWVBdfSN8CfyDgNQznyb2NwIxbVrdkHPRRuo=', 0.00, '/uploads/perfiles/b750ae17d2ba46be8eca6dd933ac8638.jpg'),
(17, 'Noelia', 'Farias', '2665234576', 'fariasnoe@example.vet', 'Las Heras 133', 'veterinario', NULL, 'ODoP9KXNWVBdfSN8CfyDgNQznyb2NwIxbVrdkHPRRuo=', 0.00, '/uploads/perfiles/83efd1f668524181a55abfbf1921d951.jpg'),
(18, 'Ramón', 'Diaz', '2664258525', 'diazramon@example.vet', 'La Toma', 'veterinario', NULL, 'ODoP9KXNWVBdfSN8CfyDgNQznyb2NwIxbVrdkHPRRuo=', 0.00, '/uploads/perfiles/4ff2d4f155054194ad1d20da471e7093.jpg'),
(19, 'Mariano', 'Lorte', '26614569785', 'lorMar@example.vet', 'Merlo', 'veterinario', NULL, 'ODoP9KXNWVBdfSN8CfyDgNQznyb2NwIxbVrdkHPRRuo=', 0.00, '/uploads/perfiles/0209d57b02c546c4a54be54d1a72fc6a.jpg');

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
-- Estructura de tabla para la tabla `veterinario`
--

CREATE TABLE `veterinario` (
  `idVeterinario` int(11) NOT NULL,
  `idUsuario` int(11) NOT NULL,
  `matricula` varchar(50) NOT NULL,
  `jurisdiccion` varchar(100) NOT NULL,
  `documentacion` varchar(255) DEFAULT NULL,
  `estadoVerificacion` varchar(20) NOT NULL,
  `fechaVerificacion` datetime(6) DEFAULT NULL,
  `observacion` varchar(500) DEFAULT NULL,
  `ofreceServiciosPagos` tinyint(1) NOT NULL DEFAULT 0,
  `ofreceVoluntariado` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `veterinario`
--

INSERT INTO `veterinario` (`idVeterinario`, `idUsuario`, `matricula`, `jurisdiccion`, `documentacion`, `estadoVerificacion`, `fechaVerificacion`, `observacion`, `ofreceServiciosPagos`, `ofreceVoluntariado`) VALUES
(1, 11, 'MAT12345', 'san luis', '/uploads/veterinarios/84280fbe87a24dbeb9cc0c35a71d362d.pdf', 'aprobado', '2026-10-15 19:45:20.475690', 'Matrícula y documentación verificadas.', 1, 1),
(2, 13, 'TEST-SL-001', 'san luis', '/uploads/veterinarios/1af4d7a0a50842d1a99914ff262a54ed.pdf', 'aprobado', '2026-10-16 00:32:02.832643', 'Matrícula y documentación verificadas.', 0, 1),
(3, 14, 'TEST-SL-002', 'san luis', '/uploads/veterinarios/8c7fb706218946ce901d060d17095a85.pdf', 'aprobado', '2026-10-16 00:32:07.700596', 'Matrícula y documentación verificadas.', 0, 1),
(4, 15, 'TEST-SL-003', 'san luis', '/uploads/veterinarios/330411b28b9a41f3b7e8d6b8a8bf0093.pdf', 'aprobado', '2026-10-16 00:32:14.473180', 'Matrícula y documentación verificadas.', 1, 0),
(5, 16, 'TEST-SL-004', 'san luis', '/uploads/veterinarios/aa1b7b0ae1c742548c572892e2981d84.pdf', 'aprobado', '2026-10-16 00:32:18.748510', 'Matrícula y documentación verificadas.', 1, 1),
(6, 17, 'MAT123-456', 'San Luis', '/uploads/veterinarios/a18dd307dcf44b31bf61ca067f09b079.pdf', 'aprobado', '2026-10-16 23:03:34.173449', NULL, 1, 0),
(7, 18, 'VET-EXAMPLE99', 'La Toma', '/uploads/veterinarios/b29f44e2a30d41239fc4a40ecffe5173.pdf', 'aprobado', '2026-10-16 23:33:41.989671', NULL, 1, 0),
(8, 19, 'MAT-67-VETF', 'Merlo', '/uploads/veterinarios/e45ab1c9dcb14b529e3a0096e70c5ab7.pdf', 'aprobado', '2026-10-17 00:24:45.831424', NULL, 0, 1);

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
('20261015145806_AgregarVeterinario', '9.0.0'),
('20261015201629_AgregarModalidadesVeterinario', '9.0.0'),
('20261024174411_AgregarCoordenadasUbicacion', '9.0.0'),
('20261026035541_AgregarDatosBusquedaMascota', '9.0.0'),
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
-- Indices de la tabla `veterinario`
--
ALTER TABLE `veterinario`
  ADD PRIMARY KEY (`idVeterinario`),
  ADD UNIQUE KEY `IX_veterinario_idUsuario` (`idUsuario`),
  ADD UNIQUE KEY `IX_veterinario_jurisdiccion_matricula` (`jurisdiccion`,`matricula`);

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
  MODIFY `idMascota` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=28;

--
-- AUTO_INCREMENT de la tabla `mascotareportefoto`
--
ALTER TABLE `mascotareportefoto`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qr`
--
ALTER TABLE `qr`
  MODIFY `idQR` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=27;

--
-- AUTO_INCREMENT de la tabla `reporte_mascota`
--
ALTER TABLE `reporte_mascota`
  MODIFY `idReporte` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `idUsuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

--
-- AUTO_INCREMENT de la tabla `validaciones`
--
ALTER TABLE `validaciones`
  MODIFY `idValidacion` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de la tabla `veterinario`
--
ALTER TABLE `veterinario`
  MODIFY `idVeterinario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

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

--
-- Filtros para la tabla `veterinario`
--
ALTER TABLE `veterinario`
  ADD CONSTRAINT `FK_veterinario_usuario_idUsuario` FOREIGN KEY (`idUsuario`) REFERENCES `usuario` (`idUsuario`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
