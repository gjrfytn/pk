-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

-- -----------------------------------------------------
-- Schema PK_DB
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema PK_DB
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `PK_DB` DEFAULT CHARACTER SET utf8 ;
USE `PK_DB` ;

-- -----------------------------------------------------
-- Table `PK_DB`.`users`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`users` (
  `login` VARCHAR(20) NOT NULL COMMENT 'Уникальный логин.',
  `password` VARCHAR(20) NOT NULL COMMENT 'Пароль.',
  `name` VARCHAR(50) NOT NULL COMMENT 'Полное имя.',
  `phone_number` VARCHAR(12) NOT NULL COMMENT 'Телефонный номер.',
  `role` ENUM('registrator', 'inspector', 'administrator') NOT NULL COMMENT 'Уровень доступа.',
  `comment` VARCHAR(140) NULL COMMENT 'Дополнительная информация.',
  PRIMARY KEY (`login`))
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
