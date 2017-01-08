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
-- Table `PK_DB`.`dictionaries`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`dictionaries` (
  `id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор справочника.',
  `name` VARCHAR(50) NOT NULL COMMENT 'Наименование справочника.',
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB
COMMENT = 'Справочники из ФИС.';


-- -----------------------------------------------------
-- Table `PK_DB`.`dictionaries_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`dictionaries_items` (
  `dictionary_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор справочника.',
  `item_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор элемента.',
  `name` VARCHAR(50) NOT NULL COMMENT 'Наименование элемента.',
  PRIMARY KEY (`dictionary_id`, `item_id`),
  CONSTRAINT `has`
    FOREIGN KEY (`dictionary_id`)
    REFERENCES `PK_DB`.`dictionaries` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Элементы справочников ФИС.';


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
  PRIMARY KEY (`uid`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC),
  INDEX `corresponds_idx` (`campaign_type_id` ASC),
  CONSTRAINT `corresponds`
    FOREIGN KEY (`campaign_type_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Приемные кампании.';


-- -----------------------------------------------------
-- Table `PK_DB`.`_campaigns_has_dictionaries_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`_campaigns_has_dictionaries_items` (
  `campaigns_uid` VARCHAR(200) NOT NULL,
  `dictionaries_items_dictionary_id` INT UNSIGNED NOT NULL,
  `dictionaries_items_item_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`campaigns_uid`, `dictionaries_items_dictionary_id`, `dictionaries_items_item_id`),
  INDEX `fk_campaigns_has_dictionaries_items_dictionaries_items1_idx` (`dictionaries_items_dictionary_id` ASC, `dictionaries_items_item_id` ASC),
  INDEX `fk_campaigns_has_dictionaries_items_campaigns1_idx` (`campaigns_uid` ASC),
  CONSTRAINT `fk_campaigns_has_dictionaries_items_campaigns1`
    FOREIGN KEY (`campaigns_uid`)
    REFERENCES `PK_DB`.`campaigns` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_campaigns_has_dictionaries_items_dictionaries_items1`
    FOREIGN KEY (`dictionaries_items_dictionary_id` , `dictionaries_items_item_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`dictionary_id` , `item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'campaigns:\nEducationForms.EducationFormID[1..n] - ИД формы обучения (справочник №14).\nEducationLevels.EducationLevelID[1..n] - ИД Уровня образования (справочник №2).';


-- -----------------------------------------------------
-- Table `PK_DB`.`admission_volumes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`admission_volumes` (
  `uid` VARCHAR(200) NOT NULL COMMENT 'Идентификатор в ИС ОО.',
  `campaign_uid` VARCHAR(200) NOT NULL COMMENT 'Идентификатор приёмной кампании (UID).',
  `education_level_id` INT UNSIGNED NOT NULL COMMENT 'ИД уровня образования (справочник №2).',
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'ИД направления подготовки (справочник №10).',
  `number_budget_o` INT UNSIGNED NOT NULL COMMENT 'Бюджетные места очной формы.',
  `number_budget_oz` INT UNSIGNED NOT NULL COMMENT 'Бюджетные места очно-заочной формы.',
  `number_budget_z` INT UNSIGNED NOT NULL COMMENT 'Бюджетные места заочной формы.',
  `number_paid_o` INT UNSIGNED NOT NULL COMMENT 'Места с оплатой обучения очной формы.',
  `number_paid_oz` INT UNSIGNED NOT NULL COMMENT 'Места с оплатой обучения очно-заочной формы.',
  `number_paid_z` INT UNSIGNED NOT NULL COMMENT 'Места с оплатой обучения заочной формы.',
  `number_target_o` INT UNSIGNED NOT NULL COMMENT 'Места целевого приема очной формы.',
  `number_target_oz` INT UNSIGNED NOT NULL COMMENT 'Места целевого приема очно-заочной формы.',
  `number_target_z` INT UNSIGNED NOT NULL COMMENT 'Места целевого приема заочной формы.',
  `number_quota_o` INT UNSIGNED NOT NULL COMMENT 'Места приёма по квоте лиц, имеющих особые права, очное обучение.',
  `number_quota_oz` INT UNSIGNED NOT NULL COMMENT 'Места приёма по квоте лиц, имеющих особые права, очно-заочное (вечернее) обучение.',
  `number_quota_z` INT UNSIGNED NOT NULL COMMENT 'Места приёма по квоте лиц, имеющих особые права, заочное обучение.',
  PRIMARY KEY (`uid`),
  INDEX `has_idx` (`campaign_uid` ASC),
  INDEX `corresponds_idx` (`education_level_id` ASC),
  INDEX `corresponds_dir_idx` (`direction_id` ASC),
  CONSTRAINT `has`
    FOREIGN KEY (`campaign_uid`)
    REFERENCES `PK_DB`.`campaigns` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_edu_l`
    FOREIGN KEY (`education_level_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_dir`
    FOREIGN KEY (`direction_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Объёмы приёма по направлению подготовки.';


-- -----------------------------------------------------
-- Table `PK_DB`.`distributed_admission_volumes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`distributed_admission_volumes` (
  `admission_volume_uid` VARCHAR(200) NOT NULL COMMENT 'Идентификатор объема приема по направлению подготовки.',
  `level_budget` INT UNSIGNED NOT NULL COMMENT 'ИД уровня бюджета (справочник №35).',
  `number_budget_o` INT UNSIGNED NOT NULL COMMENT 'Бюджетные места очной формы.',
  `number_budget_oz` INT UNSIGNED NOT NULL COMMENT 'Бюджетные места очно-заочной формы.',
  `number_budget_z` INT UNSIGNED NOT NULL COMMENT 'Бюджетные места заочной формы.',
  `number_target_o` INT UNSIGNED NOT NULL COMMENT 'Места целевого приема очной формы.',
  `number_target_oz` INT UNSIGNED NOT NULL COMMENT 'Места целевого приема очно-заочной формы.',
  `number_target_z` INT UNSIGNED NOT NULL COMMENT 'Места целевого приема заочной формы.',
  `number_quota_o` INT UNSIGNED NOT NULL COMMENT 'Места приёма по квоте лиц, имеющих особые права, очное обучение.',
  `number_quota_oz` INT UNSIGNED NOT NULL COMMENT 'Места приёма по квоте лиц, имеющих особые права, очно-заочное (вечернее) обучение.',
  `number_quota_z` INT UNSIGNED NOT NULL COMMENT 'Места приёма по квоте лиц, имеющих особые права, заочное обучение.',
  PRIMARY KEY (`admission_volume_uid`, `level_budget`),
  INDEX `fund_idx` (`level_budget` ASC),
  CONSTRAINT `has`
    FOREIGN KEY (`admission_volume_uid`)
    REFERENCES `PK_DB`.`admission_volumes` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds`
    FOREIGN KEY (`level_budget`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Объемы приема распределенные по уровню бюджета (для направления подготовки).';


-- -----------------------------------------------------
-- Table `PK_DB`.`institution_achievements`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`institution_achievements` (
  `institution_achievement_uid` VARCHAR(200) NOT NULL COMMENT 'Идентификатор в ИС ОО.',
  `name` VARCHAR(500) NOT NULL COMMENT 'Наименование индивидуального достижения.',
  `id_category` INT UNSIGNED NOT NULL COMMENT 'ИД индивидуального достижения (справочник №36).',
  `max_value` SMALLINT UNSIGNED NOT NULL COMMENT 'Максимальный балл, начисляемый за индивидуальное достижение.',
  `campaign_uid` VARCHAR(200) NOT NULL COMMENT 'Идентификатор приемной кампании.',
  PRIMARY KEY (`institution_achievement_uid`, `campaign_uid`),
  INDEX `corresponds_idx` (`id_category` ASC),
  INDEX `has_idx` (`campaign_uid` ASC),
  CONSTRAINT `corresponds`
    FOREIGN KEY (`id_category`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `has`
    FOREIGN KEY (`campaign_uid`)
    REFERENCES `PK_DB`.`campaigns` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Индивидуальные достижения, учитываемые образовательной организацией.';


-- -----------------------------------------------------
-- Table `PK_DB`.`target_organizations`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`target_organizations` (
  `uid` VARCHAR(200) NOT NULL COMMENT 'Идентификатор в ИС ОО.',
  `name` VARCHAR(250) NOT NULL COMMENT 'Наименование целевой организации.',
  PRIMARY KEY (`uid`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB
COMMENT = 'Целевые организации.';


-- -----------------------------------------------------
-- Table `PK_DB`.`orders`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`orders` (
  `uid` VARCHAR(200) NOT NULL COMMENT 'Идентификатор в ИС ОО.',
  `campaign_uid` VARCHAR(200) NOT NULL COMMENT 'UID приемной кампании.',
  `order_name` VARCHAR(200) NOT NULL COMMENT 'Наименование (текстовое описание) приказа.',
  `order_number` VARCHAR(50) NOT NULL COMMENT 'Номер приказа.',
  `order_date` DATE NOT NULL COMMENT 'Дата регистрации приказа.',
  `order_date_published` DATE NOT NULL COMMENT 'Дата фактической публикации приказа.',
  `education_form_id` INT UNSIGNED NOT NULL COMMENT 'ИД Формы обучения(Справочник 14 \"Форма обучения\").',
  `finance_source_id` INT UNSIGNED NOT NULL COMMENT 'ИД источника финансирования (Справочник 15 \"Источник финансирования\").',
  `education_level_id` INT UNSIGNED NOT NULL COMMENT 'ИД Уровня образования(Справочник 2 \"Уровень образования\").',
  `stage` INT UNSIGNED NOT NULL COMMENT 'Этап приема (В случае зачисления на места в рамках контрольных цифр (бюджет) по программам бакалавриата и программам специалитета по очной и очно-заочной формам обучения, принимает значения 1 или 2). Иначе принимает значение 0.',
  `type` ENUM('admission', 'exception') NOT NULL COMMENT 'Тип приказа (зачисление или исключение).',
  PRIMARY KEY (`uid`),
  INDEX `has_idx` (`campaign_uid` ASC),
  INDEX `corresponds_edu_f_idx` (`education_form_id` ASC),
  INDEX `corresponds_fin_s_idx` (`finance_source_id` ASC),
  INDEX `corresponds_edu_l_idx` (`education_level_id` ASC),
  CONSTRAINT `has`
    FOREIGN KEY (`campaign_uid`)
    REFERENCES `PK_DB`.`campaigns` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_edu_f`
    FOREIGN KEY (`education_form_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_fin_s`
    FOREIGN KEY (`finance_source_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_edu_l`
    FOREIGN KEY (`education_level_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Приказы.';


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
