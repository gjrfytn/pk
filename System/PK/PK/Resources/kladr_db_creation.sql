CREATE SCHEMA `kladr` DEFAULT CHARACTER SET utf8 ;
USE `kladr` ;

CREATE TABLE `houses` (
  `name` VARCHAR(75) NOT NULL,
  `code` CHAR(19) NOT NULL,
  `index` CHAR(6) NULL DEFAULT NULL,
  PRIMARY KEY (`code`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE TABLE `streets` (
  `name` VARCHAR(75) NOT NULL,
  `socr` VARCHAR(15) NOT NULL,
  `code` CHAR(17) NOT NULL,
  `index` CHAR(6) NULL DEFAULT NULL,
  PRIMARY KEY (`code`),
  INDEX `name_idx` (`name` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE TABLE `subjects` (
  `name` VARCHAR(75) NOT NULL,
  `socr` VARCHAR(15) NOT NULL,
  `code` CHAR(13) NOT NULL,
  `index` CHAR(6) NULL DEFAULT NULL,
  PRIMARY KEY (`code`),
  INDEX `name_idx` (`name` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;
