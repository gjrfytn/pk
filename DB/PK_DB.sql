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
  `name` VARCHAR(100) NOT NULL COMMENT 'Наименование справочника.',
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
  `name` VARCHAR(300) NOT NULL COMMENT 'Наименование элемента.',
  PRIMARY KEY (`dictionary_id`, `item_id`),
  INDEX `has_idx` (`dictionary_id` ASC),
  INDEX `item_id_IDX` (`item_id` ASC),
  CONSTRAINT `dictionaries_items_has`
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
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `name` VARCHAR(100) NOT NULL COMMENT 'Название.',
  `start_year` INT UNSIGNED NOT NULL COMMENT 'Год начала.',
  `end_year` INT UNSIGNED NOT NULL COMMENT 'Год окончания.',
  `status_dict_id` INT UNSIGNED NOT NULL COMMENT '34',
  `status_id` INT UNSIGNED NOT NULL COMMENT 'Статус (справочник №34)',
  `type_dict_id` INT UNSIGNED NOT NULL COMMENT '38',
  `type_id` INT UNSIGNED NOT NULL COMMENT 'Тип приёмной кампании (справочник №38)',
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC),
  INDEX `corresp_type_idx` (`type_dict_id` ASC, `type_id` ASC),
  INDEX `corresp_status_idx` (`status_dict_id` ASC, `status_id` ASC),
  CONSTRAINT `campaigns_corresp_type`
    FOREIGN KEY (`type_id` , `type_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `campaigns_corresp_status`
    FOREIGN KEY (`status_id` , `status_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Приемные кампании.';


-- -----------------------------------------------------
-- Table `PK_DB`.`_campaigns_has_dictionaries_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`_campaigns_has_dictionaries_items` (
  `campaigns_id` INT UNSIGNED NOT NULL,
  `dictionaries_items_dictionary_id` INT UNSIGNED NOT NULL,
  `dictionaries_items_item_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`campaigns_id`, `dictionaries_items_dictionary_id`, `dictionaries_items_item_id`),
  INDEX `fk_campaigns_has_dictionaries_items_dictionaries_items1_idx` (`dictionaries_items_dictionary_id` ASC, `dictionaries_items_item_id` ASC),
  INDEX `fk_campaigns_has_dictionaries_items_campaigns1_idx` (`campaigns_id` ASC),
  CONSTRAINT `fk_campaigns_has_dictionaries_items_campaigns1`
    FOREIGN KEY (`campaigns_id`)
    REFERENCES `PK_DB`.`campaigns` (`id`)
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
-- Table `PK_DB`.`institution_achievements`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`institution_achievements` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `name` VARCHAR(500) NOT NULL COMMENT 'Наименование индивидуального достижения.',
  `category_dict_id` INT UNSIGNED NOT NULL COMMENT '36',
  `category_id` INT UNSIGNED NOT NULL COMMENT 'ИД индивидуального достижения (справочник №36).',
  `max_value` SMALLINT UNSIGNED NOT NULL COMMENT 'Максимальный балл, начисляемый за индивидуальное достижение.',
  `campaign_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор приемной кампании.',
  PRIMARY KEY (`id`),
  INDEX `corresp_idx` (`category_dict_id` ASC, `category_id` ASC),
  INDEX `has_idx` (`campaign_id` ASC),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC),
  CONSTRAINT `institution_achievements_corresp`
    FOREIGN KEY (`category_id` , `category_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `institution_achievements_has`
    FOREIGN KEY (`campaign_id`)
    REFERENCES `PK_DB`.`campaigns` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Индивидуальные достижения, учитываемые образовательной организацией.';


-- -----------------------------------------------------
-- Table `PK_DB`.`target_organizations`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`target_organizations` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `name` VARCHAR(250) NOT NULL COMMENT 'Наименование целевой организации.',
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB
COMMENT = 'Целевые организации.';


-- -----------------------------------------------------
-- Table `PK_DB`.`orders`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`orders` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `campaign_id` INT UNSIGNED NOT NULL COMMENT 'UID приемной кампании.',
  `name` VARCHAR(200) NOT NULL COMMENT 'Наименование (текстовое описание) приказа.',
  `number` VARCHAR(50) NOT NULL COMMENT 'Номер приказа.',
  `registration_date` DATE NOT NULL COMMENT 'Дата регистрации приказа.',
  `publication_date` DATE NOT NULL COMMENT 'Дата фактической публикации приказа.',
  `education_form_dict_id` INT UNSIGNED NOT NULL COMMENT '14',
  `education_form_id` INT UNSIGNED NOT NULL COMMENT 'ИД Формы обучения (Справочник 14 \"Форма обучения\").',
  `finance_source_dict_id` INT UNSIGNED NOT NULL COMMENT '15',
  `finance_source_id` INT UNSIGNED NOT NULL COMMENT 'ИД источника финансирования (Справочник 15 \"Источник финансирования\").',
  `education_level_dict_id` INT UNSIGNED NOT NULL COMMENT '2',
  `education_level_id` INT UNSIGNED NOT NULL COMMENT 'ИД Уровня образования (Справочник 2 \"Уровень образования\").',
  `stage` INT UNSIGNED NOT NULL COMMENT 'Этап приема (В случае зачисления на места в рамках контрольных цифр (бюджет) по программам бакалавриата и программам специалитета по очной и очно-заочной формам обучения, принимает значения 1 или 2). Иначе принимает значение 0.',
  `type` ENUM('admission', 'exception') NOT NULL COMMENT 'Тип приказа (зачисление или исключение).',
  PRIMARY KEY (`id`),
  INDEX `has_idx` (`campaign_id` ASC),
  INDEX `corresp_edu_f_idx` (`education_form_dict_id` ASC, `education_form_id` ASC),
  INDEX `corresp_fin_s_idx` (`finance_source_dict_id` ASC, `finance_source_id` ASC),
  INDEX `corresp_edu_l_idx` (`education_level_dict_id` ASC, `education_level_id` ASC),
  CONSTRAINT `orders_has`
    FOREIGN KEY (`campaign_id`)
    REFERENCES `PK_DB`.`campaigns` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `orders_corresp_edu_f`
    FOREIGN KEY (`education_form_id` , `education_form_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `orders_corresp_fin_s`
    FOREIGN KEY (`finance_source_id` , `finance_source_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `orders_corresp_edu_l`
    FOREIGN KEY (`education_level_id` , `education_level_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Приказы.';


-- -----------------------------------------------------
-- Table `PK_DB`.`documents`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`documents` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `type` ENUM('academic_diploma', 'allow_education', 'basic_diploma', 'compatriot', 'custom', 'disability', 'edu_custom', 'high_edu_diploma', 'incopl_high_edu_diploma', 'institution', 'international_olympic', 'medical', 'middle_edu_diploma', 'olympic', 'olympic_total', 'orphan', 'phd_diploma', 'post_graduate_diploma', 'school_certificate', 'sport', 'ukraine_olympic', 'veteran', 'ege', 'gia', 'identity', 'military_card', 'student') NOT NULL COMMENT 'Тип документа:\nacademic_diploma\nallow_education\nbasic_diploma\ncompatriot\ncustom\ndisability\nedu_custom\nhigh_edu_diploma\nincopl_high_edu_diploma\ninstitution\ninternational_olympic\nmedical\nmiddle_edu_diploma\nolympic\nolympic_total\norphan\nphd_diploma\npost_graduate_diploma\nschool_certificate\nsport\nukraine_olympic\nveteran\nege\ngia\nidentity\nmilitary_card\nstudent',
  `series` VARCHAR(20) NULL COMMENT 'Серия документа.',
  `number` VARCHAR(100) NULL COMMENT 'Номер документа.',
  `date` DATE NULL COMMENT 'Дата выдачи документа.',
  `organization` VARCHAR(500) NULL COMMENT 'Организация, выдавшая документ.',
  `original_recieved_date` DATE NULL COMMENT 'Дата предоставления оригиналов документов.',
  PRIMARY KEY (`id`))
ENGINE = InnoDB
COMMENT = 'Документы.';


-- -----------------------------------------------------
-- Table `PK_DB`.`entrants`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`entrants` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `last_name` VARCHAR(250) NOT NULL COMMENT 'Фамилия.',
  `first_name` VARCHAR(250) NOT NULL COMMENT 'Имя.',
  `middle_name` VARCHAR(250) NULL COMMENT 'Отчество.',
  `gender_dict_id` INT UNSIGNED NOT NULL COMMENT '5',
  `gender_id` INT UNSIGNED NOT NULL COMMENT 'Пол (справочник №5).',
  `custom_information` VARCHAR(4000) NULL COMMENT 'Дополнительные сведения, предоставленные абитуриентом.',
  `email` VARCHAR(150) NULL COMMENT 'Электронный адрес.',
  `mail_region_dict_id` INT UNSIGNED NULL COMMENT '8',
  `mail_region_id` INT UNSIGNED NULL COMMENT 'Почтовый адрес - Регион (справочник № 8)',
  `mail_town_type_dict_id` INT UNSIGNED NULL COMMENT '41',
  `mail_town_type_id` INT UNSIGNED NULL COMMENT 'Почтовый адрес - Тип населенного пункта (справочник № 41).',
  `mail_adress` VARCHAR(500) NULL COMMENT 'Почтовый адрес - Адрес.',
  `is_from_krym` INT UNSIGNED NULL COMMENT 'UID подтверждающего документа.\nЕсли абитуриент - гражданин Крыма, иначе - NULL.',
  `home_phone` VARCHAR(10) NULL COMMENT 'Домашний телефон.',
  `mobile_phone` VARCHAR(10) NULL COMMENT 'Мобильный телефон.',
  PRIMARY KEY (`id`),
  UNIQUE INDEX `is_from_krym_UNIQUE` (`is_from_krym` ASC),
  INDEX `corresp_gend_id_idx` (`gender_dict_id` ASC, `gender_id` ASC),
  INDEX `corresp_mail_region_idx` (`mail_region_dict_id` ASC, `mail_region_id` ASC),
  INDEX `corresp_mail_town_type_idx` (`mail_town_type_dict_id` ASC, `mail_town_type_id` ASC),
  INDEX `has_krym_doc_idx` (`is_from_krym` ASC),
  CONSTRAINT `entrants_corresp_gender`
    FOREIGN KEY (`gender_id` , `gender_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrants_corresp_mail_region`
    FOREIGN KEY (`mail_region_id` , `mail_region_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrants_corresp_mail_town_type`
    FOREIGN KEY (`mail_town_type_id` , `mail_town_type_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrants_has_krym_doc`
    FOREIGN KEY (`is_from_krym`)
    REFERENCES `PK_DB`.`documents` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Абитуриенты.';


-- -----------------------------------------------------
-- Table `PK_DB`.`applications`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`applications` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `number` VARCHAR(50) NOT NULL COMMENT 'Номер заявления ОО.',
  `entrant_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор абитуриента.',
  `registration_time` DATETIME NOT NULL COMMENT 'Дата и время регистрации заявления.',
  `needs_hostel` TINYINT(1) NOT NULL COMMENT 'Признак необходимости общежития.',
  `status_dict_id` INT UNSIGNED NOT NULL COMMENT '4',
  `status_id` INT UNSIGNED NOT NULL COMMENT 'Статус заявления (справочник №4).',
  `status_comment` VARCHAR(4000) NULL COMMENT 'Комментарий к статусу заявления.',
  `language` ENUM('Английский', 'Немецкий', 'Французский') NOT NULL COMMENT 'Изучаемый язык.',
  `registrator_login` VARCHAR(20) NOT NULL COMMENT 'Логин пользователя, занёсшего заявление.',
  PRIMARY KEY (`id`),
  UNIQUE INDEX `application_number_UNIQUE` (`number` ASC),
  INDEX `has_idx` (`entrant_id` ASC),
  INDEX `corresp_idx` (`status_dict_id` ASC, `status_id` ASC),
  INDEX `applications_registrered_idx` (`registrator_login` ASC),
  CONSTRAINT `applications_has`
    FOREIGN KEY (`entrant_id`)
    REFERENCES `PK_DB`.`entrants` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_corresp`
    FOREIGN KEY (`status_id` , `status_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_registrered`
    FOREIGN KEY (`registrator_login`)
    REFERENCES `PK_DB`.`users` (`login`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Заявления.';


-- -----------------------------------------------------
-- Table `PK_DB`.`faculties`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`faculties` (
  `short_name` VARCHAR(5) NOT NULL COMMENT 'Краткое название.',
  `name` VARCHAR(75) NOT NULL COMMENT 'Название.',
  PRIMARY KEY (`short_name`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB
COMMENT = 'Факультеты.';


-- -----------------------------------------------------
-- Table `PK_DB`.`dictionary_10_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`dictionary_10_items` (
  `id` INT UNSIGNED NOT NULL COMMENT 'ИД направления.',
  `name` VARCHAR(75) NOT NULL COMMENT 'Наименование направления.',
  `code` VARCHAR(14) NOT NULL COMMENT 'Код направления.',
  `qualification_code` VARCHAR(14) NULL COMMENT 'Код квалификации.',
  `period` VARCHAR(23) NULL COMMENT 'Период обучения.',
  `ugs_code` VARCHAR(14) NULL COMMENT 'Код укрупненной группы.',
  `ugs_name` VARCHAR(75) NULL COMMENT 'Наименование укрупненной группы.',
  PRIMARY KEY (`id`))
ENGINE = InnoDB
COMMENT = 'Справочник №10 \"Направления подготовки\".';


-- -----------------------------------------------------
-- Table `PK_DB`.`directions`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`directions` (
  `faculty_short_name` VARCHAR(5) NOT NULL COMMENT 'Краткое название факультета.',
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'ID направления (справочник №10).',
  PRIMARY KEY (`faculty_short_name`, `direction_id`),
  INDEX `has_faculties_idx` (`direction_id` ASC),
  INDEX `has_dictionary_10_items_idx` (`faculty_short_name` ASC),
  CONSTRAINT `directions_has_faculties`
    FOREIGN KEY (`faculty_short_name`)
    REFERENCES `PK_DB`.`faculties` (`short_name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `directions_has_dictionary_10_items`
    FOREIGN KEY (`direction_id`)
    REFERENCES `PK_DB`.`dictionary_10_items` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Направления по факультетам.';


-- -----------------------------------------------------
-- Table `PK_DB`.`applications_entrances`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`applications_entrances` (
  `application_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор заявления.',
  `faculty_short_name` VARCHAR(5) NOT NULL COMMENT 'Факультет.',
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'Направление.',
  `edu_form_dict_id` INT UNSIGNED NOT NULL COMMENT '14',
  `edu_form_id` INT UNSIGNED NOT NULL COMMENT 'Форма обучения (справочник №14).',
  `edu_source_dict_id` INT UNSIGNED NOT NULL COMMENT '15',
  `edu_source_id` INT UNSIGNED NOT NULL COMMENT 'Источник финансирования (справочник №15).',
  `target_organization_id` INT UNSIGNED NULL COMMENT 'UID организации целевого приема.',
  `is_agreed_date` DATETIME NULL COMMENT 'Дата согласия на зачисление (необходимо передать при наличии согласия на зачисление).',
  `is_disagreed_date` DATETIME NULL COMMENT 'Дата отказа от зачисления (необходимо передать при включении заявления в приказ об исключении).',
  `is_for_spo_and_vo` TINYINT(1) NOT NULL COMMENT 'Абитуриент поступает с профильным СПО/ВО.',
  PRIMARY KEY (`application_id`, `faculty_short_name`, `direction_id`, `edu_form_dict_id`, `edu_form_id`, `edu_source_dict_id`, `edu_source_id`),
  INDEX `targets_idx` (`target_organization_id` ASC),
  INDEX `has_idx` (`application_id` ASC),
  INDEX `applies_idx` (`faculty_short_name` ASC, `direction_id` ASC),
  INDEX `corresp_edu_form_idx` (`edu_form_dict_id` ASC, `edu_form_id` ASC),
  INDEX `corresp_edu_source_idx` (`edu_source_dict_id` ASC, `edu_source_id` ASC),
  CONSTRAINT `applications_entrances_has`
    FOREIGN KEY (`application_id`)
    REFERENCES `PK_DB`.`applications` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_entrances_targets`
    FOREIGN KEY (`target_organization_id`)
    REFERENCES `PK_DB`.`target_organizations` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_entrances_applies`
    FOREIGN KEY (`faculty_short_name` , `direction_id`)
    REFERENCES `PK_DB`.`directions` (`faculty_short_name` , `direction_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_entrances_corresp_edu_form`
    FOREIGN KEY (`edu_form_dict_id` , `edu_form_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`dictionary_id` , `item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_entrances_corresp_edu_source`
    FOREIGN KEY (`edu_source_dict_id` , `edu_source_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`dictionary_id` , `item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Условия приёма заявления.';


-- -----------------------------------------------------
-- Table `PK_DB`.`diploma_docs_additional_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`diploma_docs_additional_data` (
  `document_id` INT UNSIGNED NOT NULL COMMENT 'Идентфикатор документа.',
  `registration_number` VARCHAR(100) NULL COMMENT 'Регистрационный номер.',
  `speciality_id` INT UNSIGNED NULL COMMENT 'Код направления подготовки (справочник №10).',
  `end_year` INT UNSIGNED NULL COMMENT 'Год окончания.',
  `gpa` FLOAT UNSIGNED NULL COMMENT 'Средний балл.',
  PRIMARY KEY (`document_id`),
  INDEX `corresp_idx` (`speciality_id` ASC),
  CONSTRAINT `diploma_docs_additional_data_has`
    FOREIGN KEY (`document_id`)
    REFERENCES `PK_DB`.`documents` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `diploma_docs_additional_data_corresp`
    FOREIGN KEY (`speciality_id`)
    REFERENCES `PK_DB`.`dictionary_10_items` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Дополнительная информация для документов-дипломов об образовании.';


-- -----------------------------------------------------
-- Table `PK_DB`.`identity_docs_additional_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`identity_docs_additional_data` (
  `document_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор документа.',
  `last_name` VARCHAR(250) NULL COMMENT 'Фамилия.',
  `first_name` VARCHAR(250) NULL COMMENT 'Имя.',
  `middle_name` VARCHAR(250) NULL COMMENT 'Отчество.',
  `gender_dict_id` INT UNSIGNED NULL COMMENT '5',
  `gender_id` INT UNSIGNED NULL COMMENT 'Пол (справочник №5).',
  `subdivision_code` VARCHAR(7) NULL COMMENT 'Код подразделения.',
  `type_dict_id` INT UNSIGNED NOT NULL COMMENT '22',
  `type_id` INT UNSIGNED NOT NULL COMMENT 'ИД типа документа, удостоверяющего личность (справочник №22).',
  `nationality_dict_id` INT UNSIGNED NULL COMMENT '7',
  `nationality_id` INT UNSIGNED NULL COMMENT 'ИД гражданства (справочник №7).',
  `birth_date` DATE NOT NULL COMMENT 'Дата рождения.',
  `birth_place` VARCHAR(250) NULL COMMENT 'Место рождения.',
  PRIMARY KEY (`document_id`),
  INDEX `corresp_gender_idx` (`gender_dict_id` ASC, `gender_id` ASC),
  INDEX `corresp_type_idx` (`type_dict_id` ASC, `type_id` ASC),
  INDEX `corresp_nation_idx` (`nationality_dict_id` ASC, `nationality_id` ASC),
  CONSTRAINT `identity_docs_additional_data_has`
    FOREIGN KEY (`document_id`)
    REFERENCES `PK_DB`.`documents` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `identity_docs_additional_data_corresp_gender`
    FOREIGN KEY (`gender_id` , `gender_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `identity_docs_additional_data_corresp_type`
    FOREIGN KEY (`type_id` , `type_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `identity_docs_additional_data_corresp_nation`
    FOREIGN KEY (`nationality_id` , `nationality_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Дополнительная информация для документов, удостоверяющих личность.';


-- -----------------------------------------------------
-- Table `PK_DB`.`dictionary_19_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`dictionary_19_items` (
  `olympic_id` INT UNSIGNED NOT NULL COMMENT 'ИД олимпиады.',
  `olympic_number` INT UNSIGNED NULL COMMENT 'Номер олимпиады.',
  `olympic_name` VARCHAR(200) NOT NULL COMMENT 'Имя олимпиады.',
  PRIMARY KEY (`olympic_id`))
ENGINE = InnoDB
COMMENT = 'Справочник №19 \"Олимпиады\".';


-- -----------------------------------------------------
-- Table `PK_DB`.`olympic_docs_additional_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`olympic_docs_additional_data` (
  `document_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор документа.',
  `diploma_type_dict_id` INT UNSIGNED NULL COMMENT '18',
  `diploma_type_id` INT UNSIGNED NULL COMMENT 'Тип диплома (справочник №18).',
  `olympic_id` INT UNSIGNED NULL COMMENT 'ИД олимпиады (справочник №19).',
  `class_number` INT UNSIGNED NULL COMMENT 'Класс обучения (7,8,9,10 или 11).',
  `olympic_name` VARCHAR(255) NULL COMMENT 'Наименование олимпиады.',
  `olympic_profile` VARCHAR(255) NULL COMMENT 'Профиль олимпиады.',
  `olympic_date` DATE NULL COMMENT 'Дата проведения олимпиады.',
  `olympic_place` VARCHAR(255) NULL COMMENT 'Место проведения олимпиады.',
  `country_dict_id` INT UNSIGNED NULL COMMENT '7',
  `country_id` INT UNSIGNED NULL COMMENT 'Член сборной команды (справочник № 7).',
  `profile_dict_id` INT UNSIGNED NULL COMMENT '39',
  `profile_id` INT UNSIGNED NULL COMMENT 'ИД профиля олимпиады (справочник №39).',
  `olympic_subject_dict_id` INT UNSIGNED NULL COMMENT '1',
  `olympic_subject_id` INT UNSIGNED NULL COMMENT 'ИД предмета олимпиады  (должен соответствовать профилю олимпиады) (справочник № 1).',
  `ege_subject_dict_id` INT UNSIGNED NULL COMMENT '1',
  `ege_subject_id` INT UNSIGNED NULL COMMENT 'ИД предмета, по которому будет осуществляться проверка ЕГЭ (справочник № 1).',
  PRIMARY KEY (`document_id`),
  INDEX `corresp_dip_type_idx` (`diploma_type_dict_id` ASC, `diploma_type_id` ASC),
  INDEX `corresp_country_idx` (`country_dict_id` ASC, `country_id` ASC),
  INDEX `corresp_profile_idx` (`profile_dict_id` ASC, `profile_id` ASC),
  INDEX `corresp_ol_subj_idx` (`olympic_subject_dict_id` ASC, `olympic_subject_id` ASC),
  INDEX `corresp_ege_subj_idx` (`ege_subject_dict_id` ASC, `ege_subject_id` ASC),
  INDEX `corresp_olympic_idx` (`olympic_id` ASC),
  CONSTRAINT `olympic_docs_additional_data_has`
    FOREIGN KEY (`document_id`)
    REFERENCES `PK_DB`.`documents` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `olympic_docs_additional_data_corresp_dip_type`
    FOREIGN KEY (`diploma_type_id` , `diploma_type_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `olympic_docs_additional_data_corresp_olympic`
    FOREIGN KEY (`olympic_id`)
    REFERENCES `PK_DB`.`dictionary_19_items` (`olympic_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `olympic_docs_additional_data_corresp_country`
    FOREIGN KEY (`country_id` , `country_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `olympic_docs_additional_data_corresp_profile`
    FOREIGN KEY (`profile_id` , `profile_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `olympic_docs_additional_data_corresp_ol_subj`
    FOREIGN KEY (`olympic_subject_id` , `olympic_subject_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `olympic_docs_additional_data_corresp_ege_subj`
    FOREIGN KEY (`ege_subject_id` , `ege_subject_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Дополнительная информация для документов по олимпиадам.';


-- -----------------------------------------------------
-- Table `PK_DB`.`other_docs_additional_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`other_docs_additional_data` (
  `document_id` INT UNSIGNED NOT NULL COMMENT 'Идентфикатор документа.',
  `name` VARCHAR(1000) NULL COMMENT 'Наименование документа.',
  `dictionaries_dictionary_id` INT UNSIGNED NULL COMMENT '45, 33, 43, 42, 23, 44',
  `dictionaries_item_id` INT UNSIGNED NULL COMMENT 'ИД элемента справочника. Для разных документов:\nveteran - VeteranCategoryID - Тип документа, подтверждающего принадлежность к ветеранам боевых действий (справочник № 45).\ninstitution: DocumentTypeID - Тип документа (справочник №33).\nsport: SportCategoryID - Тип диплома в области спорта (справочник № 43).\norphan: OrphanCategoryID - Тип документа, подтверждающего сиротство (справочник № 42).\ndisability: DisabilityTypeID - Группа инвалидности (справочник №23).\ncompatriot: CompariotCategoryID - Тип документа, подтверждающего принадлежность к соотечественникам (справочник № 44).',
  `text_data` VARCHAR(4000) NULL COMMENT 'Текстовые данные. Для разных документов:\ncustom, sport: AdditionalInfo - Дополнительные сведения.\nedu_custom: DocumentTypeNameText - Наименование документа.',
  `year` INT UNSIGNED NULL COMMENT 'Для документа типа ege - Год выдачи свидетельства.',
  PRIMARY KEY (`document_id`),
  INDEX `corresp_idx` (`dictionaries_dictionary_id` ASC, `dictionaries_item_id` ASC),
  CONSTRAINT `other_docs_additional_data_has`
    FOREIGN KEY (`document_id`)
    REFERENCES `PK_DB`.`documents` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `other_docs_additional_data_corresp`
    FOREIGN KEY (`dictionaries_item_id` , `dictionaries_dictionary_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Дополнительная информация для остальных документов.';


-- -----------------------------------------------------
-- Table `PK_DB`.`olympic_docs_subjects`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`olympic_docs_subjects` (
  `olympic_docs_ad_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор документа.',
  `subject_dict_id` INT UNSIGNED NOT NULL COMMENT '39',
  `subject_id` INT UNSIGNED NOT NULL COMMENT 'ИД профильной дисциплины  (справочник №39).',
  PRIMARY KEY (`olympic_docs_ad_id`, `subject_dict_id`, `subject_id`),
  INDEX `corresp_idx` (`subject_dict_id` ASC, `subject_id` ASC),
  INDEX `has_idx` (`olympic_docs_ad_id` ASC),
  CONSTRAINT `olympic_docs_subjects_has`
    FOREIGN KEY (`olympic_docs_ad_id`)
    REFERENCES `PK_DB`.`olympic_docs_additional_data` (`document_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `olympic_docs_subjects_corresp`
    FOREIGN KEY (`subject_id` , `subject_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Профильные предметы олимпиады.';


-- -----------------------------------------------------
-- Table `PK_DB`.`documents_subjects_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`documents_subjects_data` (
  `document_id` INT UNSIGNED NOT NULL COMMENT 'Идентфикатор документа.',
  `subject_dict_id` INT UNSIGNED NOT NULL COMMENT '1',
  `subject_id` INT UNSIGNED NOT NULL COMMENT 'ИД дисциплины (справочник №1).',
  `value` INT UNSIGNED NOT NULL COMMENT 'Балл.',
  PRIMARY KEY (`document_id`, `subject_dict_id`, `subject_id`),
  INDEX `corresp_idx` (`subject_dict_id` ASC, `subject_id` ASC),
  INDEX `has_idx` (`document_id` ASC),
  CONSTRAINT `documents_subjects_data_has`
    FOREIGN KEY (`document_id`)
    REFERENCES `PK_DB`.`documents` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `documents_subjects_data_corresp`
    FOREIGN KEY (`subject_id` , `subject_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Дисциплины документов.';


-- -----------------------------------------------------
-- Table `PK_DB`.`application_common_benefits`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`application_common_benefits` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `application_id` INT UNSIGNED NOT NULL COMMENT 'Идентифиактор заявления.',
  `document_type_dict_id` INT UNSIGNED NOT NULL COMMENT '31',
  `document_type_id` INT UNSIGNED NOT NULL COMMENT 'ИД типа документа-основания (справочник №31).',
  `reason_document_id` INT UNSIGNED NOT NULL COMMENT 'Сведения о документе-основании (идентификатор документа).',
  `allow_education_document_id` INT UNSIGNED NULL COMMENT 'Заключение об отсутствии противопоказаний для обучения (идентфикатор документа).\nЕсли в качестве основания выступает документ типа disability или medical, иначе NULL.',
  `benefit_kind_dict_id` INT UNSIGNED NOT NULL COMMENT '30',
  `benefit_kind_id` INT UNSIGNED NOT NULL COMMENT 'ИД вида льготы (справочник №30).',
  PRIMARY KEY (`id`),
  INDEX `has_idx` (`application_id` ASC),
  INDEX `corresp_doc_type_idx` (`document_type_dict_id` ASC, `document_type_id` ASC),
  INDEX `confirms_idx` (`reason_document_id` ASC),
  INDEX `allows_education_idx` (`allow_education_document_id` ASC),
  INDEX `corresp_bnf_kind_idx` (`benefit_kind_dict_id` ASC, `benefit_kind_id` ASC),
  CONSTRAINT `application_common_benefits_has`
    FOREIGN KEY (`application_id`)
    REFERENCES `PK_DB`.`applications` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `application_common_benefits_corresp_doc_type`
    FOREIGN KEY (`document_type_id` , `document_type_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `application_common_benefits_confirms`
    FOREIGN KEY (`reason_document_id`)
    REFERENCES `PK_DB`.`documents` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `application_common_benefits_allows_education`
    FOREIGN KEY (`allow_education_document_id`)
    REFERENCES `PK_DB`.`documents` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `application_common_benefits_corresp_bnf_kind`
    FOREIGN KEY (`benefit_kind_id` , `benefit_kind_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Льготы, предоставленные абитуриенту.';


-- -----------------------------------------------------
-- Table `PK_DB`.`_applications_has_documents`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`_applications_has_documents` (
  `applications_id` INT UNSIGNED NOT NULL,
  `documents_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`applications_id`, `documents_id`),
  INDEX `fk_applications_has_documents_documents1_idx` (`documents_id` ASC),
  INDEX `fk_applications_has_documents_applications1_idx` (`applications_id` ASC),
  CONSTRAINT `fk_applications_has_documents_applications1`
    FOREIGN KEY (`applications_id`)
    REFERENCES `PK_DB`.`applications` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_applications_has_documents_documents1`
    FOREIGN KEY (`documents_id`)
    REFERENCES `PK_DB`.`documents` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'applications:\nApplicationDocuments - Документы, приложенные к заявлению.';


-- -----------------------------------------------------
-- Table `PK_DB`.`individual_achievements`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`individual_achievements` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор индивидуального достижения, учитываемого в заявлении.',
  `application_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор заявления.',
  `institution_achievement_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор достижения, указанный в приемной кампании.',
  `mark` INT UNSIGNED NULL COMMENT 'Балл за достижение.',
  `document_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор документа, подтверждающего индивидуальное достижение.',
  PRIMARY KEY (`id`),
  INDEX `has_idx` (`application_id` ASC),
  INDEX `gets_idx` (`institution_achievement_id` ASC),
  INDEX `confirms_idx` (`document_id` ASC),
  CONSTRAINT `individual_achievements_has`
    FOREIGN KEY (`application_id`)
    REFERENCES `PK_DB`.`applications` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `individual_achievements_gets`
    FOREIGN KEY (`institution_achievement_id`)
    REFERENCES `PK_DB`.`institution_achievements` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `individual_achievements_confirms`
    FOREIGN KEY (`document_id`)
    REFERENCES `PK_DB`.`documents` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Индивидуальные достижения.';


-- -----------------------------------------------------
-- Table `PK_DB`.`dictionary_olympic_profiles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`dictionary_olympic_profiles` (
  `olympic_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор.',
  `profile_dict_id` INT UNSIGNED NOT NULL COMMENT '39',
  `profile_id` INT UNSIGNED NOT NULL COMMENT 'ИД профиля олимпиады (справочник №39).',
  `level_dict_id` INT UNSIGNED NULL COMMENT '3',
  `level_id` INT UNSIGNED NULL COMMENT 'ИД уровня олимпиады (справочник №3).',
  PRIMARY KEY (`olympic_id`, `profile_dict_id`, `profile_id`),
  INDEX `corresp_profile_idx` (`profile_dict_id` ASC, `profile_id` ASC),
  INDEX `corresp_level_idx` (`level_dict_id` ASC, `level_id` ASC),
  INDEX `has_idx` (`olympic_id` ASC),
  CONSTRAINT `dictionary_olympic_profiles_has`
    FOREIGN KEY (`olympic_id`)
    REFERENCES `PK_DB`.`dictionary_19_items` (`olympic_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `dictionary_olympic_profiles_corresp_profile`
    FOREIGN KEY (`profile_id` , `profile_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `dictionary_olympic_profiles_corresp_level`
    FOREIGN KEY (`level_id` , `level_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Профили олимпиады из справочника №10.';


-- -----------------------------------------------------
-- Table `PK_DB`.`_dictionary_olympic_profiles_has_dictionaries_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`_dictionary_olympic_profiles_has_dictionaries_items` (
  `dictionary_olympic_profiles_olympic_id` INT UNSIGNED NOT NULL,
  `dictionary_olympic_profiles_profile_dict_id` INT UNSIGNED NOT NULL,
  `dictionary_olympic_profiles_profile_id` INT UNSIGNED NOT NULL,
  `dictionaries_items_dictionary_id` INT UNSIGNED NOT NULL,
  `dictionaries_items_item_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`dictionary_olympic_profiles_olympic_id`, `dictionary_olympic_profiles_profile_dict_id`, `dictionary_olympic_profiles_profile_id`, `dictionaries_items_dictionary_id`, `dictionaries_items_item_id`),
  INDEX `fk_dictionary_olympic_profiles_has_dictionaries_items_dict2_idx` (`dictionaries_items_dictionary_id` ASC, `dictionaries_items_item_id` ASC),
  INDEX `fk_dictionary_olympic_profiles_has_dictionaries_items_dict1_idx` (`dictionary_olympic_profiles_olympic_id` ASC, `dictionary_olympic_profiles_profile_dict_id` ASC, `dictionary_olympic_profiles_profile_id` ASC),
  CONSTRAINT `fk_dictionary_olympic_profiles_has_dictionaries_items_diction1`
    FOREIGN KEY (`dictionary_olympic_profiles_olympic_id` , `dictionary_olympic_profiles_profile_dict_id` , `dictionary_olympic_profiles_profile_id`)
    REFERENCES `PK_DB`.`dictionary_olympic_profiles` (`olympic_id` , `profile_dict_id` , `profile_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_dictionary_olympic_profiles_has_dictionaries_items_diction2`
    FOREIGN KEY (`dictionaries_items_item_id` , `dictionaries_items_dictionary_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'dictionary_olympic_profiles:\nSubjects.SubjectID[1..n] - ИД предмета (справочник №1).';


-- -----------------------------------------------------
-- Table `PK_DB`.`profiles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`profiles` (
  `faculty_short_name` VARCHAR(5) NOT NULL COMMENT 'Факультет.',
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'Направление.',
  `name` VARCHAR(100) NOT NULL COMMENT 'Название профиля.',
  PRIMARY KEY (`faculty_short_name`, `direction_id`, `name`),
  INDEX `has_idx` (`faculty_short_name` ASC, `direction_id` ASC),
  CONSTRAINT `profiles_has`
    FOREIGN KEY (`faculty_short_name` , `direction_id`)
    REFERENCES `PK_DB`.`directions` (`faculty_short_name` , `direction_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Профили обучения по направлениям.';


-- -----------------------------------------------------
-- Table `PK_DB`.`campaigns_faculties_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`campaigns_faculties_data` (
  `campaign_id` INT UNSIGNED NOT NULL COMMENT 'Кампания.',
  `faculty_short_name` VARCHAR(5) NOT NULL COMMENT 'Факультет.',
  `hostel_places` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество мест в общежитии.',
  PRIMARY KEY (`campaign_id`, `faculty_short_name`),
  INDEX `fk_faculties_has_campaigns_campaigns1_idx` (`campaign_id` ASC),
  INDEX `fk_faculties_has_campaigns_faculties1_idx` (`faculty_short_name` ASC),
  CONSTRAINT `fk_faculties_has_campaigns_faculties1`
    FOREIGN KEY (`faculty_short_name`)
    REFERENCES `PK_DB`.`faculties` (`short_name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_faculties_has_campaigns_campaigns1`
    FOREIGN KEY (`campaign_id`)
    REFERENCES `PK_DB`.`campaigns` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Данные кампаний по факультетам.';


-- -----------------------------------------------------
-- Table `PK_DB`.`campaigns_directions_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`campaigns_directions_data` (
  `campaign_id` INT UNSIGNED NOT NULL COMMENT 'Кампания.',
  `direction_faculty` VARCHAR(5) NOT NULL COMMENT 'Факультет направления.',
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'ID направления (Справочник №10).',
  `places_budget_o` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество бюджетных очных мест.',
  `places_budget_oz` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество бюджетных вечерних мест.',
  `places_budget_z` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество бюджетных заочных мест.',
  `places_target_o` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество целевых очных мест.',
  `places_target_oz` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество целевых вечерних мест.',
  `places_target_z` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество целевых заочных мест.',
  `places_quota_o` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество квотированных очных мест.',
  `places_quota_oz` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество квотированных вечерних мест.',
  `places_quota_z` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество квотированных заочных мест.',
  PRIMARY KEY (`campaign_id`, `direction_faculty`, `direction_id`),
  INDEX `has_camp_idx` (`campaign_id` ASC),
  INDEX `has_fac_dir_idx` (`direction_faculty` ASC, `direction_id` ASC),
  CONSTRAINT `campaigns_directions_data_has_fac_dir`
    FOREIGN KEY (`direction_id` , `direction_faculty`)
    REFERENCES `PK_DB`.`directions` (`direction_id` , `faculty_short_name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `campaigns_directions_data_has_camp`
    FOREIGN KEY (`campaign_id`)
    REFERENCES `PK_DB`.`campaigns` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Данные кампаний по направлениям.';


-- -----------------------------------------------------
-- Table `PK_DB`.`campaigns_profiles_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`campaigns_profiles_data` (
  `campaigns_id` INT UNSIGNED NOT NULL COMMENT 'Кампания.',
  `profiles_direction_faculty` VARCHAR(5) NOT NULL COMMENT 'Факультет направления.',
  `profiles_direction_id` INT UNSIGNED NOT NULL COMMENT 'ID направления.',
  `profiles_name` VARCHAR(100) NOT NULL COMMENT 'Профиль.',
  `places_paid_o` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество платных очных мест.',
  `places_paid_oz` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество платных вечерних мест.',
  `places_paid_z` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество платных заочных мест.',
  PRIMARY KEY (`campaigns_id`, `profiles_direction_faculty`, `profiles_direction_id`, `profiles_name`),
  INDEX `fk_profiles_has_campaigns_campaigns1_idx` (`campaigns_id` ASC),
  CONSTRAINT `fk_profiles_has_campaigns_profiles1`
    FOREIGN KEY (`profiles_direction_faculty` , `profiles_direction_id` , `profiles_name`)
    REFERENCES `PK_DB`.`profiles` (`faculty_short_name` , `direction_id` , `name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_profiles_has_campaigns_campaigns1`
    FOREIGN KEY (`campaigns_id`)
    REFERENCES `PK_DB`.`campaigns` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Данные кампаний по профилям.';


-- -----------------------------------------------------
-- Table `PK_DB`.`entrance_tests`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`entrance_tests` (
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'ID направления (Справочник №10).',
  `subject_dict_id` INT UNSIGNED NOT NULL COMMENT '1',
  `subject_id` INT UNSIGNED NOT NULL COMMENT 'ID дисциплины (Справочник №1).',
  `priority` SMALLINT UNSIGNED NOT NULL COMMENT 'Приоритет.',
  PRIMARY KEY (`direction_id`, `subject_dict_id`, `subject_id`),
  INDEX `has_idx` (`direction_id` ASC),
  INDEX `corresp_idx` (`subject_dict_id` ASC, `subject_id` ASC),
  CONSTRAINT `dir_entrance_tests_has`
    FOREIGN KEY (`direction_id`)
    REFERENCES `PK_DB`.`dictionary_10_items` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `dir_entrance_tests_corresp`
    FOREIGN KEY (`subject_dict_id` , `subject_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`dictionary_id` , `item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Вступительные испытания по направлениям.';


-- -----------------------------------------------------
-- Table `PK_DB`.`examinations`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`examinations` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор.',
  `subject_dict_id` INT UNSIGNED NOT NULL COMMENT '1',
  `subject_id` INT UNSIGNED NOT NULL COMMENT 'ID дисциплины (справочник №1).',
  `date` DATE NOT NULL COMMENT 'Дата проведения.',
  `reg_start_date` DATE NOT NULL COMMENT 'Дата регистрации заявления, начиная с которой абитуриенты попадают в этот поток.',
  `reg_end_date` DATE NOT NULL COMMENT 'Дата регистрации заявления, после которой абитуриенты не попадают в этот поток.',
  PRIMARY KEY (`id`),
  INDEX `examinations_corresponds_idx` (`subject_dict_id` ASC, `subject_id` ASC),
  CONSTRAINT `examinations_corresponds`
    FOREIGN KEY (`subject_dict_id` , `subject_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`dictionary_id` , `item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Внутренние экзамены.';


-- -----------------------------------------------------
-- Table `PK_DB`.`examinations_audiences`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`examinations_audiences` (
  `examination_id` INT UNSIGNED NOT NULL COMMENT 'ID экзамена.',
  `number` VARCHAR(5) NOT NULL COMMENT 'Номер аудитории.',
  `capacity` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество мест.',
  PRIMARY KEY (`examination_id`, `number`),
  INDEX `has` (`examination_id` ASC),
  CONSTRAINT `examinations_audiences_has`
    FOREIGN KEY (`examination_id`)
    REFERENCES `PK_DB`.`examinations` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Экзаменационные аудитории.';


-- -----------------------------------------------------
-- Table `PK_DB`.`entrants_examinations_marks`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`entrants_examinations_marks` (
  `entrant_id` INT UNSIGNED NOT NULL COMMENT 'UID абитуриента.',
  `examination_id` INT UNSIGNED NOT NULL COMMENT 'ID экзамена.',
  `mark` SMALLINT NOT NULL COMMENT 'Оценка.',
  PRIMARY KEY (`entrant_id`, `examination_id`),
  INDEX `entr_exam_marks_has_exam_idx` (`examination_id` ASC),
  INDEX `entr_exam_marks_has_entr_idx` (`entrant_id` ASC),
  CONSTRAINT `entr_exam_marks_has_entr`
    FOREIGN KEY (`entrant_id`)
    REFERENCES `PK_DB`.`entrants` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entr_exam_marks_has_exam`
    FOREIGN KEY (`examination_id`)
    REFERENCES `PK_DB`.`examinations` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Оценки абитуриентов по внутренним экзаменам.';


-- -----------------------------------------------------
-- Table `PK_DB`.`constants`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`constants` (
  `min_math_mark` SMALLINT UNSIGNED NOT NULL COMMENT 'Минимальный балл по математике.')
ENGINE = InnoDB
COMMENT = 'Константы.';

USE `PK_DB` ;

-- -----------------------------------------------------
-- procedure get_campaign_edu_forms
-- -----------------------------------------------------

DELIMITER $$
USE `PK_DB`$$
CREATE PROCEDURE `get_campaign_edu_forms` (IN id INT UNSIGNED)
BEGIN
SELECT 
    name
FROM
    _campaigns_has_dictionaries_items
        JOIN
    dictionaries_items ON _campaigns_has_dictionaries_items.dictionaries_items_dictionary_id = dictionaries_items.dictionary_id
        AND _campaigns_has_dictionaries_items.dictionaries_items_item_id = dictionaries_items.item_id
WHERE
    dictionaries_items_dictionary_id = 14
        AND campaigns_id = id;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure get_entrants_marks
-- -----------------------------------------------------

DELIMITER $$
USE `PK_DB`$$
CREATE PROCEDURE `get_entrants_marks` (id INT UNSIGNED)
BEGIN
SELECT id, CONCAT_WS(' ',last_name,first_name,middle_name), IF(mark!=-1,mark,'неявка') FROM
entrants_examinations_marks
JOIN
entrants ON entrant_id=entrants.id
WHERE examination_id=id;
END$$

DELIMITER ;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
