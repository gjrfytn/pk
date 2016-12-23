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
ENGINE = InnoDB
COMMENT = 'Пользователи ИС ПК МАДИ.';


-- -----------------------------------------------------
-- Table `PK_DB`.`campaigns`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`campaigns` (
  `uid` VARCHAR(200) NOT NULL COMMENT 'Идентификатор в ИС ОО.',
  `name` VARCHAR(100) NOT NULL COMMENT 'Название.',
  `year_start` INT UNSIGNED NOT NULL COMMENT 'Год начала.',
  `year_end` INT UNSIGNED NOT NULL COMMENT 'Год окончания.',
  `status_id` INT UNSIGNED NOT NULL COMMENT 'Статус (справочник №34)',
  `campaign_type_id` INT UNSIGNED NOT NULL COMMENT 'Тип приёмной кампании (справочник №38)',
  PRIMARY KEY (`uid`))
ENGINE = InnoDB
COMMENT = 'Приемные кампании.';


-- -----------------------------------------------------
-- Table `PK_DB`.`dic_14`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`dic_14` (
  `id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор формы обучения.',
  `name` VARCHAR(25) NOT NULL COMMENT 'Наименование формы обучения.',
  PRIMARY KEY (`id`),
  UNIQUE INDEX `dic_14_edu_form_name_UNIQUE` (`name` ASC))
ENGINE = InnoDB
COMMENT = 'Справочник №14 \"Формы обучения\".';


-- -----------------------------------------------------
-- Table `PK_DB`.`dic_2`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`dic_2` (
  `id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор уровня образования.',
  `name` VARCHAR(25) NOT NULL COMMENT 'Наименование уровня образования.',
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB
COMMENT = 'Справочник №2 \"Уровни образовния\".';


-- -----------------------------------------------------
-- Table `PK_DB`.`campaigns_has_dic_14`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`campaigns_has_dic_14` (
  `campaigns_uid` VARCHAR(200) NOT NULL,
  `dic_14_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`campaigns_uid`, `dic_14_id`),
  INDEX `fk_campaigns_has_dic_14_dic_141_idx` (`dic_14_id` ASC),
  INDEX `fk_campaigns_has_dic_14_campaigns_idx` (`campaigns_uid` ASC),
  CONSTRAINT `fk_campaigns_has_dic_14_campaigns`
    FOREIGN KEY (`campaigns_uid`)
    REFERENCES `PK_DB`.`campaigns` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_campaigns_has_dic_14_dic_141`
    FOREIGN KEY (`dic_14_id`)
    REFERENCES `PK_DB`.`dic_14` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `PK_DB`.`campaigns_has_dic_2`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`campaigns_has_dic_2` (
  `campaigns_uid` VARCHAR(200) NOT NULL,
  `dic_2_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`campaigns_uid`, `dic_2_id`),
  INDEX `fk_campaigns_has_dic_2_dic_21_idx` (`dic_2_id` ASC),
  INDEX `fk_campaigns_has_dic_2_campaigns1_idx` (`campaigns_uid` ASC),
  CONSTRAINT `fk_campaigns_has_dic_2_campaigns1`
    FOREIGN KEY (`campaigns_uid`)
    REFERENCES `PK_DB`.`campaigns` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_campaigns_has_dic_2_dic_21`
    FOREIGN KEY (`dic_2_id`)
    REFERENCES `PK_DB`.`dic_2` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
