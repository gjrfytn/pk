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
  `uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `name` VARCHAR(100) NOT NULL COMMENT 'Название.',
  `start_year` INT UNSIGNED NOT NULL COMMENT 'Год начала.',
  `end_year` INT UNSIGNED NOT NULL COMMENT 'Год окончания.',
  `status_dict_id` INT UNSIGNED NOT NULL,
  `status_id` INT UNSIGNED NOT NULL COMMENT 'Статус (справочник №34)',
  `type_dict_id` INT UNSIGNED NOT NULL,
  `type_id` INT UNSIGNED NOT NULL COMMENT 'Тип приёмной кампании (справочник №38)',
  PRIMARY KEY (`uid`),
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
  `campaigns_uid` INT UNSIGNED NOT NULL,
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
-- Table `PK_DB`.`admission_volumes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`admission_volumes` (
  `uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `campaign_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор приёмной кампании (UID).',
  `education_level_dict_id` INT UNSIGNED NOT NULL,
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
  INDEX `corresp_edu_l_idx` (`education_level_dict_id` ASC, `education_level_id` ASC),
  INDEX `corresp_dir_idx` (`direction_id` ASC),
  CONSTRAINT `admission_volumes_has`
    FOREIGN KEY (`campaign_uid`)
    REFERENCES `PK_DB`.`campaigns` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `admission_volumes_corresp_edu_l`
    FOREIGN KEY (`education_level_id` , `education_level_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `admission_volumes_corresp_dir`
    FOREIGN KEY (`direction_id`)
    REFERENCES `PK_DB`.`dictionary_10_items` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Объёмы приёма по направлению подготовки.';


-- -----------------------------------------------------
-- Table `PK_DB`.`distributed_admission_volumes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`distributed_admission_volumes` (
  `admission_volume_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор объема приема по направлению подготовки.',
  `budget_level_dict_id` INT UNSIGNED NOT NULL,
  `budget_level_id` INT UNSIGNED NOT NULL COMMENT 'ИД уровня бюджета (справочник №35).',
  `number_budget_o` INT UNSIGNED NOT NULL COMMENT 'Бюджетные места очной формы.',
  `number_budget_oz` INT UNSIGNED NOT NULL COMMENT 'Бюджетные места очно-заочной формы.',
  `number_budget_z` INT UNSIGNED NOT NULL COMMENT 'Бюджетные места заочной формы.',
  `number_target_o` INT UNSIGNED NOT NULL COMMENT 'Места целевого приема очной формы.',
  `number_target_oz` INT UNSIGNED NOT NULL COMMENT 'Места целевого приема очно-заочной формы.',
  `number_target_z` INT UNSIGNED NOT NULL COMMENT 'Места целевого приема заочной формы.',
  `number_quota_o` INT UNSIGNED NOT NULL COMMENT 'Места приёма по квоте лиц, имеющих особые права, очное обучение.',
  `number_quota_oz` INT UNSIGNED NOT NULL COMMENT 'Места приёма по квоте лиц, имеющих особые права, очно-заочное (вечернее) обучение.',
  `number_quota_z` INT UNSIGNED NOT NULL COMMENT 'Места приёма по квоте лиц, имеющих особые права, заочное обучение.',
  PRIMARY KEY (`admission_volume_uid`, `budget_level_dict_id`, `budget_level_id`),
  INDEX `corresp_idx` (`budget_level_dict_id` ASC, `budget_level_id` ASC),
  INDEX `has_idx` (`admission_volume_uid` ASC),
  CONSTRAINT `distributed_admission_volumes_has`
    FOREIGN KEY (`admission_volume_uid`)
    REFERENCES `PK_DB`.`admission_volumes` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `distributed_admission_volumes_corresp`
    FOREIGN KEY (`budget_level_id` , `budget_level_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Объемы приема распределенные по уровню бюджета (для направления подготовки).';


-- -----------------------------------------------------
-- Table `PK_DB`.`institution_achievements`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`institution_achievements` (
  `institution_achievement_uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `name` VARCHAR(500) NOT NULL COMMENT 'Наименование индивидуального достижения.',
  `category_dict_id` INT UNSIGNED NOT NULL,
  `category_id` INT UNSIGNED NOT NULL COMMENT 'ИД индивидуального достижения (справочник №36).',
  `max_value` SMALLINT UNSIGNED NOT NULL COMMENT 'Максимальный балл, начисляемый за индивидуальное достижение.',
  `campaign_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор приемной кампании.',
  PRIMARY KEY (`institution_achievement_uid`),
  INDEX `corresp_idx` (`category_dict_id` ASC, `category_id` ASC),
  INDEX `has_idx` (`campaign_uid` ASC),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC),
  CONSTRAINT `institution_achievements_corresp`
    FOREIGN KEY (`category_id` , `category_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `institution_achievements_has`
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
  `uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `name` VARCHAR(250) NOT NULL COMMENT 'Наименование целевой организации.',
  PRIMARY KEY (`uid`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB
COMMENT = 'Целевые организации.';


-- -----------------------------------------------------
-- Table `PK_DB`.`orders`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`orders` (
  `uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `campaign_uid` INT UNSIGNED NOT NULL COMMENT 'UID приемной кампании.',
  `name` VARCHAR(200) NOT NULL COMMENT 'Наименование (текстовое описание) приказа.',
  `number` VARCHAR(50) NOT NULL COMMENT 'Номер приказа.',
  `registration_date` DATE NOT NULL COMMENT 'Дата регистрации приказа.',
  `publication_date` DATE NOT NULL COMMENT 'Дата фактической публикации приказа.',
  `education_form_dict_id` INT UNSIGNED NOT NULL,
  `education_form_id` INT UNSIGNED NOT NULL COMMENT 'ИД Формы обучения (Справочник 14 \"Форма обучения\").',
  `finance_source_dict_id` INT UNSIGNED NOT NULL,
  `finance_source_id` INT UNSIGNED NOT NULL COMMENT 'ИД источника финансирования (Справочник 15 \"Источник финансирования\").',
  `education_level_dict_id` INT UNSIGNED NOT NULL,
  `education_level_id` INT UNSIGNED NOT NULL COMMENT 'ИД Уровня образования (Справочник 2 \"Уровень образования\").',
  `stage` INT UNSIGNED NOT NULL COMMENT 'Этап приема (В случае зачисления на места в рамках контрольных цифр (бюджет) по программам бакалавриата и программам специалитета по очной и очно-заочной формам обучения, принимает значения 1 или 2). Иначе принимает значение 0.',
  `type` ENUM('admission', 'exception') NOT NULL COMMENT 'Тип приказа (зачисление или исключение).',
  PRIMARY KEY (`uid`),
  INDEX `has_idx` (`campaign_uid` ASC),
  INDEX `corresp_edu_f_idx` (`education_form_dict_id` ASC, `education_form_id` ASC),
  INDEX `corresp_fin_s_idx` (`finance_source_dict_id` ASC, `finance_source_id` ASC),
  INDEX `corresp_edu_l_idx` (`education_level_dict_id` ASC, `education_level_id` ASC),
  CONSTRAINT `orders_has`
    FOREIGN KEY (`campaign_uid`)
    REFERENCES `PK_DB`.`campaigns` (`uid`)
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
-- Table `PK_DB`.`competitive_groups`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`competitive_groups` (
  `uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `campaign_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор приемной кампании (UID).',
  `name` VARCHAR(250) NOT NULL COMMENT 'Наименование конкурса.',
  `education_level_dict_id` INT UNSIGNED NOT NULL,
  `education_level_id` INT UNSIGNED NOT NULL COMMENT 'ИД уровня образования (справочник №2).',
  `education_source_dict_id` INT UNSIGNED NOT NULL,
  `education_source_id` INT UNSIGNED NOT NULL COMMENT 'ИД источника финансирования (справочник №15).',
  `education_form_dict_id` INT UNSIGNED NOT NULL,
  `education_form_id` INT UNSIGNED NOT NULL COMMENT 'ИД формы обучения (справочник №14).',
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'ИД направления подготовки (справочник №10).',
  `is_for_krym` TINYINT(1) NOT NULL COMMENT 'Конкурс для граждан Крыма.',
  `is_additional` TINYINT(1) NOT NULL COMMENT 'Конкурс для дополнительного набора.',
  `places_number` INT UNSIGNED NOT NULL COMMENT 'Количество мест в конкурсной группе.',
  PRIMARY KEY (`uid`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC),
  INDEX `has_idx` (`campaign_uid` ASC),
  INDEX `corresp_edu_lvl_idx` (`education_level_dict_id` ASC, `education_level_id` ASC),
  INDEX `corresp_edu_src_idx` (`education_source_dict_id` ASC, `education_source_id` ASC),
  INDEX `corresp_edu_form_idx` (`education_form_dict_id` ASC, `education_form_id` ASC),
  INDEX `corresp_direction_idx` (`direction_id` ASC),
  CONSTRAINT `competitive_groups_has`
    FOREIGN KEY (`campaign_uid`)
    REFERENCES `PK_DB`.`campaigns` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `competitive_groups_corresp_edu_lvl`
    FOREIGN KEY (`education_level_id` , `education_level_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `competitive_groups_corresp_edu_src`
    FOREIGN KEY (`education_source_id` , `education_source_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `competitive_groups_corresp_edu_form`
    FOREIGN KEY (`education_form_id` , `education_form_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `competitive_groups_corresp_direction`
    FOREIGN KEY (`direction_id`)
    REFERENCES `PK_DB`.`dictionary_10_items` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Конкурсные группы (конкурсы).';


-- -----------------------------------------------------
-- Table `PK_DB`.`edu_programs`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`edu_programs` (
  `uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `name` VARCHAR(200) NOT NULL COMMENT 'Наименование ОП.',
  `code` VARCHAR(10) NOT NULL COMMENT 'Код ОП.',
  PRIMARY KEY (`uid`))
ENGINE = InnoDB
COMMENT = 'Образовательные программы.';


-- -----------------------------------------------------
-- Table `PK_DB`.`_competitive_groups_has_edu_programs`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`_competitive_groups_has_edu_programs` (
  `competitive_groups_uid` INT UNSIGNED NOT NULL,
  `edu_programs_uid` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`competitive_groups_uid`, `edu_programs_uid`),
  INDEX `fk_competitive_groups_has_edu_programs_edu_programs1_idx` (`edu_programs_uid` ASC),
  INDEX `fk_competitive_groups_has_edu_programs_competitive_groups1_idx` (`competitive_groups_uid` ASC),
  CONSTRAINT `fk_competitive_groups_has_edu_programs_competitive_groups1`
    FOREIGN KEY (`competitive_groups_uid`)
    REFERENCES `PK_DB`.`competitive_groups` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_competitive_groups_has_edu_programs_edu_programs1`
    FOREIGN KEY (`edu_programs_uid`)
    REFERENCES `PK_DB`.`edu_programs` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'competitive_groups:\nEduPrograms.EduProgram[1..n] - Образовательные программы.';


-- -----------------------------------------------------
-- Table `PK_DB`.`target_organizations_adm_volumes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`target_organizations_adm_volumes` (
  `competitive_group_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор конкурсной группы.',
  `target_organization_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор организации в ИС ОО.',
  `edu_form_dict_id` INT UNSIGNED NOT NULL,
  `edu_form_id` INT UNSIGNED NOT NULL COMMENT 'Форма обучения.',
  `places_number` INT UNSIGNED NOT NULL COMMENT 'Количество мест.',
  PRIMARY KEY (`competitive_group_uid`, `target_organization_uid`),
  INDEX `has_idx` (`competitive_group_uid` ASC),
  INDEX `corresp_edu_f_idx` (`edu_form_dict_id` ASC, `edu_form_id` ASC),
  INDEX `corresp_t_org_idx` (`target_organization_uid` ASC),
  CONSTRAINT `target_organizations_adm_volumes_has`
    FOREIGN KEY (`competitive_group_uid`)
    REFERENCES `PK_DB`.`competitive_groups` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `target_organizations_adm_volumes_corresp_t_org`
    FOREIGN KEY (`target_organization_uid`)
    REFERENCES `PK_DB`.`target_organizations` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `target_organizations_adm_volumes_corresp_edu_f`
    FOREIGN KEY (`edu_form_id` , `edu_form_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Сведения о целевом наборе от организаций.';


-- -----------------------------------------------------
-- Table `PK_DB`.`entrance_tests`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`entrance_tests` (
  `uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `type_dict_id` INT UNSIGNED NOT NULL,
  `type_id` INT UNSIGNED NOT NULL COMMENT 'Тип вступительного испытания (справочник №11).',
  `min_score` SMALLINT UNSIGNED NOT NULL COMMENT 'Минимальное количество баллов.',
  `priority` INT UNSIGNED NOT NULL COMMENT 'Приоритет вступительного испытания.',
  `subject_dict_id` INT UNSIGNED NOT NULL,
  `subject_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор дисциплины вступительного испытания (справочник №1).',
  `is_for_spo_and_vo` INT UNSIGNED NULL COMMENT 'UID заменяемого испытания.\nЕсли испытание для поступающих на основании профильного СПО/ВО, иначе - NULL.',
  `competitive_group_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор конкурсной группы.',
  PRIMARY KEY (`uid`),
  INDEX `corresp_type_idx` (`type_dict_id` ASC, `type_id` ASC),
  INDEX `corresp_subject_idx` (`subject_dict_id` ASC, `subject_id` ASC),
  INDEX `replaces_test_idx` (`is_for_spo_and_vo` ASC),
  INDEX `has_idx` (`competitive_group_uid` ASC),
  CONSTRAINT `entrance_tests_corresp_type`
    FOREIGN KEY (`type_id` , `type_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrance_tests_corresp_subject`
    FOREIGN KEY (`subject_id` , `subject_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrance_tests_replaces_test`
    FOREIGN KEY (`is_for_spo_and_vo`)
    REFERENCES `PK_DB`.`entrance_tests` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrance_tests_has`
    FOREIGN KEY (`competitive_group_uid`)
    REFERENCES `PK_DB`.`competitive_groups` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Вступительные испытания конкурсов.';


-- -----------------------------------------------------
-- Table `PK_DB`.`entrance_benefits`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`entrance_benefits` (
  `uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `benefit_kind_dict_id` INT UNSIGNED NOT NULL,
  `benefit_kind_id` INT UNSIGNED NOT NULL COMMENT 'Вид льготы (справочник №30).',
  `is_for_all_olympics` TINYINT(1) NOT NULL COMMENT 'Флаг действия льготы для всех олимпиад.',
  `is_creative` TINYINT(1) NULL COMMENT 'Творческие олимпиады.\nNULL, если льгота относится к вступительному испытанию.',
  `is_athletic` TINYINT(1) NULL COMMENT 'Олимпиады в области спорта.\nNULL, если льгота относится к вступительному испытанию.',
  `level_dict_id_for_all_olympics` INT UNSIGNED NOT NULL,
  `level_id_for_all_olympics` INT UNSIGNED NOT NULL COMMENT 'Уровни для всех олимпиад (справочник №3).',
  `class_dict_id_for_all_olympics` INT UNSIGNED NOT NULL,
  `class_id_for_all_olympics` INT UNSIGNED NOT NULL COMMENT 'Классы для всех олимпиад (справочник №40).',
  `competitive_group_uid` INT UNSIGNED NULL COMMENT 'Идентификатор конкурсной группы.\nNULL, если льгота относится к вступительному испытанию.',
  `min_ege_mark` INT UNSIGNED NULL COMMENT 'Минимальный балл ЕГЭ, при котором можно использовать льготу.\nNULL, если льгота относится к конкурсной группе.',
  `entrance_test_id` INT UNSIGNED NULL COMMENT 'Идентификатор вступительного испытания.\nNULL, если льгота относится к конкурсной группе.',
  PRIMARY KEY (`uid`),
  INDEX `corresp_bnf_kind_idx` (`benefit_kind_dict_id` ASC, `benefit_kind_id` ASC),
  INDEX `corresp_lvl_for_all_idx` (`level_dict_id_for_all_olympics` ASC, `level_id_for_all_olympics` ASC),
  INDEX `corresp_cls_for_all_idx` (`class_dict_id_for_all_olympics` ASC, `class_id_for_all_olympics` ASC),
  INDEX `has_comp_gr_idx` (`competitive_group_uid` ASC),
  INDEX `has_entr_test_idx` (`entrance_test_id` ASC),
  CONSTRAINT `entrance_benefits_corresp_bnf_kind`
    FOREIGN KEY (`benefit_kind_id` , `benefit_kind_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrance_benefits_corresp_lvl_for_all`
    FOREIGN KEY (`level_id_for_all_olympics` , `level_dict_id_for_all_olympics`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrance_benefits_corresp_cls_for_all`
    FOREIGN KEY (`class_id_for_all_olympics` , `class_dict_id_for_all_olympics`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrance_benefits_has_comp_gr`
    FOREIGN KEY (`competitive_group_uid`)
    REFERENCES `PK_DB`.`competitive_groups` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrance_benefits_has_entr_test`
    FOREIGN KEY (`entrance_test_id`)
    REFERENCES `PK_DB`.`entrance_tests` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Льготы (без в.и.) (олимпиада школьников). Выступает в роли либо общей льготы, либо льготы к вступительному испытанию.';


-- -----------------------------------------------------
-- Table `PK_DB`.`_entrance_benefits_has_dictionaries_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`_entrance_benefits_has_dictionaries_items` (
  `common_benefits_uid` INT UNSIGNED NOT NULL,
  `dictionaries_items_dictionary_id` INT UNSIGNED NOT NULL,
  `dictionaries_items_item_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`common_benefits_uid`, `dictionaries_items_dictionary_id`, `dictionaries_items_item_id`),
  INDEX `fk_common_benefits_has_dictionaries_items_dictionaries_item_idx` (`dictionaries_items_dictionary_id` ASC, `dictionaries_items_item_id` ASC),
  INDEX `fk_common_benefits_has_dictionaries_items_common_benefits1_idx` (`common_benefits_uid` ASC),
  CONSTRAINT `fk_entrance_benefits_has_dictionaries_items_entrance_benefits1`
    FOREIGN KEY (`common_benefits_uid`)
    REFERENCES `PK_DB`.`entrance_benefits` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_entrance_benefits_has_dictionaries_items_dictionaries_items1`
    FOREIGN KEY (`dictionaries_items_dictionary_id` , `dictionaries_items_item_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`dictionary_id` , `item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'common_benefits:\nOlympicDiplomTypes.OlympicDiplomTypeID[1..n] - ИД типа диплома (справочник №18).\nProfileForAllOlympics.ProfileID[1..n] - ИД профиля олимпиады (справочник №39).';


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
-- Table `PK_DB`.`benefits_olympics_levels`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`benefits_olympics_levels` (
  `benefit_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор льготы.',
  `olympic_id` INT UNSIGNED NOT NULL COMMENT 'ИД олимпиады (справочник №19).',
  `level_dict_id` INT UNSIGNED NOT NULL,
  `level_id` INT UNSIGNED NOT NULL COMMENT 'Уровни олимпиады (справочник №3).',
  `class_dict_id` INT UNSIGNED NOT NULL,
  `class_id` INT UNSIGNED NOT NULL COMMENT 'Классы олимпиады (справочник №40).',
  PRIMARY KEY (`benefit_uid`, `olympic_id`),
  INDEX `corresp_lvl_idx` (`level_dict_id` ASC, `level_id` ASC),
  INDEX `corresp_class_idx` (`class_dict_id` ASC, `class_id` ASC),
  INDEX `has_idx` (`benefit_uid` ASC),
  INDEX `corresp_olymp` (`olympic_id` ASC),
  CONSTRAINT `benefits_olympics_levels_corresp_olymp`
    FOREIGN KEY (`olympic_id`)
    REFERENCES `PK_DB`.`dictionary_19_items` (`olympic_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `benefits_olympics_levels_corresp_lvl`
    FOREIGN KEY (`level_id` , `level_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `benefits_olympics_levels_corresp_class`
    FOREIGN KEY (`class_id` , `class_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `benefits_olympics_levels_has`
    FOREIGN KEY (`benefit_uid`)
    REFERENCES `PK_DB`.`entrance_benefits` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Параметры олимпиад, определяющие льготу.';


-- -----------------------------------------------------
-- Table `PK_DB`.`_benefits_olympics_levels_has_dictionaries_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`_benefits_olympics_levels_has_dictionaries_items` (
  `benefit_olympics_levels_olympic_id` INT UNSIGNED NOT NULL,
  `benefit_olympics_levels_common_benefit_uid` INT UNSIGNED NOT NULL,
  `dictionaries_items_dictionary_id` INT UNSIGNED NOT NULL,
  `dictionaries_items_item_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`benefit_olympics_levels_olympic_id`, `benefit_olympics_levels_common_benefit_uid`, `dictionaries_items_dictionary_id`, `dictionaries_items_item_id`),
  INDEX `fk_olymps_lvls_has_dicts_items_dicts_items1_idx` (`dictionaries_items_dictionary_id` ASC, `dictionaries_items_item_id` ASC),
  INDEX `fk_bnfs_olymps_lvls_has_dicts_items_olymps_lvls1_idx` (`benefit_olympics_levels_olympic_id` ASC, `benefit_olympics_levels_common_benefit_uid` ASC),
  CONSTRAINT `fk_bnfs_olymps_lvls_has_dicts_items_olymps_lvls1`
    FOREIGN KEY (`benefit_olympics_levels_olympic_id` , `benefit_olympics_levels_common_benefit_uid`)
    REFERENCES `PK_DB`.`benefits_olympics_levels` (`olympic_id` , `benefit_uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_olymps_lvls_has_dicts_items_dicts_items1`
    FOREIGN KEY (`dictionaries_items_item_id` , `dictionaries_items_dictionary_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'olympics_levels:\nProfiles.ProfileID[1..n] - ИД профиля олимпиады (справочник №39).';


-- -----------------------------------------------------
-- Table `PK_DB`.`benefits_min_ege_marks`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`benefits_min_ege_marks` (
  `benefit_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор льготы.',
  `subject_dict_id` INT UNSIGNED NOT NULL,
  `subject_id` INT UNSIGNED NOT NULL COMMENT 'ИД дисциплины (справочник №1).',
  `min_mark` INT UNSIGNED NOT NULL COMMENT 'Минимальная оценка по предмету.',
  PRIMARY KEY (`benefit_uid`, `subject_dict_id`, `subject_id`),
  INDEX `corresp_idx` (`subject_dict_id` ASC, `subject_id` ASC),
  INDEX `has_idx` (`benefit_uid` ASC),
  CONSTRAINT `benefits_min_ege_marks_has`
    FOREIGN KEY (`benefit_uid`)
    REFERENCES `PK_DB`.`entrance_benefits` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `benefits_min_ege_marks_corresp`
    FOREIGN KEY (`subject_id` , `subject_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Минимальная оценка по предметам для льготы.';


-- -----------------------------------------------------
-- Table `PK_DB`.`documents`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`documents` (
  `uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `type` ENUM('academic_diploma', 'allow_education', 'basic_diploma', 'compatriot', 'custom', 'disability', 'edu_custom', 'high_edu_diploma', 'incopl_high_edu_diploma', 'institution', 'international_olympic', 'medical', 'middle_edu_diploma', 'olympic', 'olympic_total', 'orphan', 'phd_diploma', 'post_graduate_diploma', 'school_certificate', 'sport', 'ukraine_olympic', 'veteran', 'ege', 'gia', 'identity', 'military_card', 'student') NOT NULL COMMENT 'Тип документа:\nacademic_diploma\nallow_education\nbasic_diploma\ncompatriot\ncustom\ndisability\nedu_custom\nhigh_edu_diploma\nincopl_high_edu_diploma\ninstitution\ninternational_olympic\nmedical\nmiddle_edu_diploma\nolympic\nolympic_total\norphan\nphd_diploma\npost_graduate_diploma\nschool_certificate\nsport\nukraine_olympic\nveteran\nege\ngia\nidentity\nmilitary_card\nstudent',
  `series` VARCHAR(20) NULL COMMENT 'Серия документа.',
  `number` VARCHAR(100) NULL COMMENT 'Номер документа.',
  `date` DATE NULL COMMENT 'Дата выдачи документа.',
  `organization` VARCHAR(500) NULL COMMENT 'Организация, выдавшая документ.',
  `original_recieved_date` DATE NULL COMMENT 'Дата предоставления оригиналов документов.',
  PRIMARY KEY (`uid`))
ENGINE = InnoDB
COMMENT = 'Документы.';


-- -----------------------------------------------------
-- Table `PK_DB`.`entrants`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`entrants` (
  `uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `last_name` VARCHAR(250) NOT NULL COMMENT 'Фамилия.',
  `first_name` VARCHAR(250) NOT NULL COMMENT 'Имя.',
  `middle_name` VARCHAR(250) NULL COMMENT 'Отчество.',
  `gender_dict_id` INT UNSIGNED NOT NULL,
  `gender_id` INT UNSIGNED NOT NULL COMMENT 'Пол (справочник №5).',
  `custom_information` VARCHAR(4000) NULL COMMENT 'Дополнительные сведения, предоставленные абитуриентом.',
  `email` VARCHAR(150) NULL COMMENT 'Электронный адрес.',
  `mail_region_dict_id` INT UNSIGNED NULL,
  `mail_region_id` INT UNSIGNED NULL COMMENT 'Почтовый адрес - Регион (справочник № 8)',
  `mail_town_type_dict_id` INT UNSIGNED NULL,
  `mail_town_type_id` INT UNSIGNED NULL COMMENT 'Почтовый адрес - Тип населенного пункта (справочник № 41).',
  `mail_adress` VARCHAR(500) NULL COMMENT 'Почтовый адрес - Адрес.',
  `is_from_krym` INT UNSIGNED NULL COMMENT 'UID подтверждающего документа.\nЕсли абитуриент - гражданин Крыма, иначе - NULL.',
  PRIMARY KEY (`uid`),
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
    REFERENCES `PK_DB`.`documents` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Абитуриенты.';


-- -----------------------------------------------------
-- Table `PK_DB`.`applications`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`applications` (
  `uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `number` VARCHAR(50) NOT NULL COMMENT 'Номер заявления ОО.',
  `entrant_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор абитуриента.',
  `registration_time` DATETIME NOT NULL COMMENT 'Дата и время регистрации заявления.',
  `needs_hostel` TINYINT(1) NOT NULL COMMENT 'Признак необходимости общежития.',
  `status_dict_id` INT UNSIGNED NOT NULL,
  `status_id` INT UNSIGNED NOT NULL COMMENT 'Статус заявления (справочник №4).',
  `status_comment` VARCHAR(4000) NULL COMMENT 'Комментарий к статусу заявления.',
  PRIMARY KEY (`uid`),
  UNIQUE INDEX `application_number_UNIQUE` (`number` ASC),
  INDEX `has_idx` (`entrant_uid` ASC),
  INDEX `corresp_idx` (`status_dict_id` ASC, `status_id` ASC),
  CONSTRAINT `applications_has`
    FOREIGN KEY (`entrant_uid`)
    REFERENCES `PK_DB`.`entrants` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_corresp`
    FOREIGN KEY (`status_id` , `status_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Заявления.';


-- -----------------------------------------------------
-- Table `PK_DB`.`applications_entrances`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`applications_entrances` (
  `application_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор заявления.',
  `competitive_group_uid` INT UNSIGNED NOT NULL COMMENT 'UID конкурсной группы.',
  `target_organization_uid` INT UNSIGNED NULL COMMENT 'UID организации целевого приема.',
  `is_agreed_date` DATETIME NULL COMMENT 'Дата согласия на зачисление (необходимо передать при наличии согласия на зачисление).',
  `is_disagreed_date` DATETIME NULL COMMENT 'Дата отказа от зачисления (необходимо передать при включении заявления в приказ об исключении).',
  `is_for_spo_and_vo` TINYINT(1) NOT NULL COMMENT 'Абитуриент поступает с профильным СПО/ВО.',
  PRIMARY KEY (`application_uid`, `competitive_group_uid`),
  INDEX `enters_idx` (`competitive_group_uid` ASC),
  INDEX `targets_idx` (`target_organization_uid` ASC),
  INDEX `has_idx` (`application_uid` ASC),
  CONSTRAINT `applications_entrances_has`
    FOREIGN KEY (`application_uid`)
    REFERENCES `PK_DB`.`applications` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_entrances_enters`
    FOREIGN KEY (`competitive_group_uid`)
    REFERENCES `PK_DB`.`competitive_groups` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_entrances_targets`
    FOREIGN KEY (`target_organization_uid`)
    REFERENCES `PK_DB`.`target_organizations` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Условия приёма заявления.';


-- -----------------------------------------------------
-- Table `PK_DB`.`diploma_docs_additional_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`diploma_docs_additional_data` (
  `document_uid` INT UNSIGNED NOT NULL COMMENT 'Идентфикатор документа.',
  `registration_number` VARCHAR(100) NULL COMMENT 'Регистрационный номер.',
  `speciality_id` INT UNSIGNED NULL COMMENT 'Код направления подготовки (справочник №10).',
  `end_year` INT UNSIGNED NULL COMMENT 'Год окончания.',
  `gpa` FLOAT UNSIGNED NULL COMMENT 'Средний балл.',
  PRIMARY KEY (`document_uid`),
  INDEX `corresp_idx` (`speciality_id` ASC),
  CONSTRAINT `diploma_docs_additional_data_has`
    FOREIGN KEY (`document_uid`)
    REFERENCES `PK_DB`.`documents` (`uid`)
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
  `document_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор документа.',
  `last_name` VARCHAR(250) NULL COMMENT 'Фамилия.',
  `first_name` VARCHAR(250) NULL COMMENT 'Имя.',
  `middle_name` VARCHAR(250) NULL COMMENT 'Отчество.',
  `gender_dict_id` INT UNSIGNED NULL,
  `gender_id` INT UNSIGNED NULL COMMENT 'Пол (справочник №5).',
  `subdivision_code` VARCHAR(7) NULL COMMENT 'Код подразделения.',
  `type_dict_id` INT UNSIGNED NOT NULL,
  `type_id` INT UNSIGNED NOT NULL COMMENT 'ИД типа документа, удостоверяющего личность (справочник №22).',
  `nationality_dict_id` INT UNSIGNED NULL,
  `nationality_id` INT UNSIGNED NULL COMMENT 'ИД гражданства (справочник №7).',
  `birth_date` DATE NOT NULL COMMENT 'Дата рождения.',
  `birth_place` VARCHAR(250) NULL COMMENT 'Место рождения.',
  PRIMARY KEY (`document_uid`),
  INDEX `corresp_gender_idx` (`gender_dict_id` ASC, `gender_id` ASC),
  INDEX `corresp_type_idx` (`type_dict_id` ASC, `type_id` ASC),
  INDEX `corresp_nation_idx` (`nationality_dict_id` ASC, `nationality_id` ASC),
  CONSTRAINT `identity_docs_additional_data_has`
    FOREIGN KEY (`document_uid`)
    REFERENCES `PK_DB`.`documents` (`uid`)
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
-- Table `PK_DB`.`olympic_docs_additional_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`olympic_docs_additional_data` (
  `document_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор документа.',
  `diploma_type_dict_id` INT UNSIGNED NULL,
  `diploma_type_id` INT UNSIGNED NULL COMMENT 'Тип диплома (справочник №18).',
  `olympic_id` INT UNSIGNED NULL COMMENT 'ИД олимпиады (справочник №19).',
  `class_number` INT UNSIGNED NULL COMMENT 'Класс обучения (7,8,9,10 или 11).',
  `olympic_name` VARCHAR(255) NULL COMMENT 'Наименование олимпиады.',
  `olympic_profile` VARCHAR(255) NULL COMMENT 'Профиль олимпиады.',
  `olympic_date` DATE NULL COMMENT 'Дата проведения олимпиады.',
  `olympic_place` VARCHAR(255) NULL COMMENT 'Место проведения олимпиады.',
  `country_dict_id` INT UNSIGNED NULL,
  `country_id` INT UNSIGNED NULL COMMENT 'Член сборной команды (справочник № 7).',
  `profile_dict_id` INT UNSIGNED NULL,
  `profile_id` INT UNSIGNED NULL COMMENT 'ИД профиля олимпиады (справочник №39).',
  `olympic_subject_dict_id` INT UNSIGNED NULL,
  `olympic_subject_id` INT UNSIGNED NULL COMMENT 'ИД предмета олимпиады  (должен соответствовать профилю олимпиады) (справочник № 1).',
  `ege_subject_dict_id` INT UNSIGNED NULL,
  `ege_subject_id` INT UNSIGNED NULL COMMENT 'ИД предмета, по которому будет осуществляться проверка ЕГЭ (справочник № 1).',
  PRIMARY KEY (`document_uid`),
  INDEX `corresp_dip_type_idx` (`diploma_type_dict_id` ASC, `diploma_type_id` ASC),
  INDEX `corresp_country_idx` (`country_dict_id` ASC, `country_id` ASC),
  INDEX `corresp_profile_idx` (`profile_dict_id` ASC, `profile_id` ASC),
  INDEX `corresp_ol_subj_idx` (`olympic_subject_dict_id` ASC, `olympic_subject_id` ASC),
  INDEX `corresp_ege_subj_idx` (`ege_subject_dict_id` ASC, `ege_subject_id` ASC),
  INDEX `corresp_olympic_idx` (`olympic_id` ASC),
  CONSTRAINT `olympic_docs_additional_data_has`
    FOREIGN KEY (`document_uid`)
    REFERENCES `PK_DB`.`documents` (`uid`)
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
  `document_uid` INT UNSIGNED NOT NULL COMMENT 'Идентфикатор документа.',
  `name` VARCHAR(1000) NULL COMMENT 'Наименование документа.',
  `dictionaries_dictionary_id` INT UNSIGNED NULL,
  `dictionaries_item_id` INT UNSIGNED NULL COMMENT 'ИД элемента справочника. Для разных документов:\nveteran - VeteranCategoryID - Тип документа, подтверждающего принадлежность к ветеранам боевых действий (справочник № 45).\ninstitution: DocumentTypeID - Тип документа (справочник №33).\nsport: SportCategoryID - Тип диплома в области спорта (справочник № 43).\norphan: OrphanCategoryID - Тип документа, подтверждающего сиротство (справочник № 42).\ndisability: DisabilityTypeID - Группа инвалидности (справочник №23).\ncompatriot: CompariotCategoryID - Тип документа, подтверждающего принадлежность к соотечественникам (справочник № 44).',
  `text_data` VARCHAR(4000) NULL COMMENT 'Текстовые данные. Для разных документов:\ncustom, sport: AdditionalInfo - Дополнительные сведения.\nedu_custom: DocumentTypeNameText - Наименование документа.',
  `year` INT UNSIGNED NULL COMMENT 'Для документа типа ege - Год выдачи свидетельства.',
  PRIMARY KEY (`document_uid`),
  INDEX `corresp_idx` (`dictionaries_dictionary_id` ASC, `dictionaries_item_id` ASC),
  CONSTRAINT `other_docs_additional_data_has`
    FOREIGN KEY (`document_uid`)
    REFERENCES `PK_DB`.`documents` (`uid`)
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
  `subject_dict_id` INT UNSIGNED NOT NULL,
  `subject_id` INT UNSIGNED NOT NULL COMMENT 'ИД профильной дисциплины  (справочник №39).',
  PRIMARY KEY (`olympic_docs_ad_id`, `subject_dict_id`, `subject_id`),
  INDEX `corresp_idx` (`subject_dict_id` ASC, `subject_id` ASC),
  INDEX `has_idx` (`olympic_docs_ad_id` ASC),
  CONSTRAINT `olympic_docs_subjects_has`
    FOREIGN KEY (`olympic_docs_ad_id`)
    REFERENCES `PK_DB`.`olympic_docs_additional_data` (`document_uid`)
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
  `document_uid` INT UNSIGNED NOT NULL COMMENT 'Идентфикатор документа.',
  `subject_dict_id` INT UNSIGNED NOT NULL,
  `subject_id` INT UNSIGNED NOT NULL COMMENT 'ИД дисциплины (справочник №1).',
  `value` INT UNSIGNED NOT NULL COMMENT 'Балл.',
  PRIMARY KEY (`document_uid`, `subject_dict_id`, `subject_id`),
  INDEX `corresp_idx` (`subject_dict_id` ASC, `subject_id` ASC),
  INDEX `has_idx` (`document_uid` ASC),
  CONSTRAINT `documents_subjects_data_has`
    FOREIGN KEY (`document_uid`)
    REFERENCES `PK_DB`.`documents` (`uid`)
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
  `uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `application_uid` INT UNSIGNED NOT NULL COMMENT 'Идентифиактор заявления.',
  `competitive_group_uid` INT UNSIGNED NOT NULL COMMENT 'UID конкурса для льготы.',
  `document_type_dict_id` INT UNSIGNED NOT NULL,
  `document_type_id` INT UNSIGNED NOT NULL COMMENT 'ИД типа документа-основания (справочник №31).',
  `reason_document_uid` INT UNSIGNED NOT NULL COMMENT 'Сведения о документе-основании (идентификатор документа).',
  `allow_education_document_uid` INT UNSIGNED NULL COMMENT 'Заключение об отсутствии противопоказаний для обучения (идентфикатор документа).\nЕсли в качестве основания выступает документ типа disability или medical, иначе NULL.',
  `benefit_kind_dict_id` INT UNSIGNED NOT NULL,
  `benefit_kind_id` INT UNSIGNED NOT NULL COMMENT 'ИД вида льготы (справочник №30).',
  PRIMARY KEY (`uid`),
  INDEX `has_idx` (`application_uid` ASC),
  INDEX `applies_idx` (`competitive_group_uid` ASC),
  INDEX `corresp_doc_type_idx` (`document_type_dict_id` ASC, `document_type_id` ASC),
  INDEX `confirms_idx` (`reason_document_uid` ASC),
  INDEX `allows_education_idx` (`allow_education_document_uid` ASC),
  INDEX `corresp_bnf_kind_idx` (`benefit_kind_dict_id` ASC, `benefit_kind_id` ASC),
  CONSTRAINT `application_common_benefits_has`
    FOREIGN KEY (`application_uid`)
    REFERENCES `PK_DB`.`applications` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `application_common_benefits_applies`
    FOREIGN KEY (`competitive_group_uid`)
    REFERENCES `PK_DB`.`competitive_groups` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `application_common_benefits_corresp_doc_type`
    FOREIGN KEY (`document_type_id` , `document_type_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `application_common_benefits_confirms`
    FOREIGN KEY (`reason_document_uid`)
    REFERENCES `PK_DB`.`documents` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `application_common_benefits_allows_education`
    FOREIGN KEY (`allow_education_document_uid`)
    REFERENCES `PK_DB`.`documents` (`uid`)
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
  `applications_uid` INT UNSIGNED NOT NULL,
  `documents_uid` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`applications_uid`, `documents_uid`),
  INDEX `fk_applications_has_documents_documents1_idx` (`documents_uid` ASC),
  INDEX `fk_applications_has_documents_applications1_idx` (`applications_uid` ASC),
  CONSTRAINT `fk_applications_has_documents_applications1`
    FOREIGN KEY (`applications_uid`)
    REFERENCES `PK_DB`.`applications` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_applications_has_documents_documents1`
    FOREIGN KEY (`documents_uid`)
    REFERENCES `PK_DB`.`documents` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'applications:\nApplicationDocuments - Документы, приложенные к заявлению.';


-- -----------------------------------------------------
-- Table `PK_DB`.`entrance_tests_results`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`entrance_tests_results` (
  `uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `application_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор заявления.',
  `result_value` INT UNSIGNED NOT NULL COMMENT 'Балл.',
  `result_source_type_dict_id` INT UNSIGNED NOT NULL,
  `result_source_type_id` INT UNSIGNED NOT NULL COMMENT 'ИД основания для оценки (справочник №6).',
  `entrance_test_subject_dict_id` INT UNSIGNED NOT NULL,
  `entrance_test_subject_id` INT UNSIGNED NOT NULL COMMENT 'ИД дисциплины (справочник №1).',
  `entrance_test_type_dict_id` INT UNSIGNED NOT NULL,
  `entrance_test_type_id` INT UNSIGNED NOT NULL COMMENT 'ИД типа вступительного испытания (справочник №11).',
  `competitive_group_uid` INT UNSIGNED NOT NULL COMMENT 'UID конкурсной группы.',
  `result_document_uid` INT UNSIGNED NULL COMMENT 'Сведения об основании для оценки (идентификатор документа).',
  `is_distant` VARCHAR(200) NULL COMMENT 'ВИ с использованием дистанционных технологий.\nМесто сдачи ВИ, если использовались, иначе NULL.',
  `is_disabled` INT UNSIGNED NULL COMMENT 'ВИ с созданием специальных условий.\nUID подтверждающего документа, если создавались, иначе NULL.',
  PRIMARY KEY (`uid`),
  INDEX `corresp_src_type_idx` (`result_source_type_dict_id` ASC, `result_source_type_id` ASC),
  INDEX `corresp_subject_idx` (`entrance_test_subject_dict_id` ASC, `entrance_test_subject_id` ASC),
  INDEX `corresp_test_type_idx` (`entrance_test_type_dict_id` ASC, `entrance_test_type_id` ASC),
  INDEX `applies_idx` (`competitive_group_uid` ASC),
  INDEX `confirms_idx` (`result_document_uid` ASC),
  INDEX `confirms_disabled_idx` (`is_disabled` ASC),
  INDEX `has_idx` (`application_uid` ASC),
  CONSTRAINT `entrance_tests_results_corresp_src_type`
    FOREIGN KEY (`result_source_type_id` , `result_source_type_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrance_tests_results_corresp_subject`
    FOREIGN KEY (`entrance_test_subject_id` , `entrance_test_subject_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrance_tests_results_corresp_test_type`
    FOREIGN KEY (`entrance_test_type_id` , `entrance_test_type_dict_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrance_tests_results_applies`
    FOREIGN KEY (`competitive_group_uid`)
    REFERENCES `PK_DB`.`competitive_groups` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrance_tests_results_confirms`
    FOREIGN KEY (`result_document_uid`)
    REFERENCES `PK_DB`.`documents` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrance_tests_results_confirms_disabled`
    FOREIGN KEY (`is_disabled`)
    REFERENCES `PK_DB`.`documents` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entrance_tests_results_has`
    FOREIGN KEY (`application_uid`)
    REFERENCES `PK_DB`.`applications` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Результаты вступительных испытаний.';


-- -----------------------------------------------------
-- Table `PK_DB`.`individual_achievements`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`individual_achievements` (
  `uid` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор индивидуального достижения, учитываемого в заявлении.',
  `application_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор заявления.',
  `institution_achievement_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор достижения, указанный в приемной кампании.',
  `mark` INT UNSIGNED NULL COMMENT 'Балл за достижение.',
  `document_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор документа, подтверждающего индивидуальное достижение.',
  PRIMARY KEY (`uid`),
  INDEX `has_idx` (`application_uid` ASC),
  INDEX `gets_idx` (`institution_achievement_uid` ASC),
  INDEX `confirms_idx` (`document_uid` ASC),
  CONSTRAINT `individual_achievements_has`
    FOREIGN KEY (`application_uid`)
    REFERENCES `PK_DB`.`applications` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `individual_achievements_gets`
    FOREIGN KEY (`institution_achievement_uid`)
    REFERENCES `PK_DB`.`institution_achievements` (`institution_achievement_uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `individual_achievements_confirms`
    FOREIGN KEY (`document_uid`)
    REFERENCES `PK_DB`.`documents` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Индивидуальные достижения.';


-- -----------------------------------------------------
-- Table `PK_DB`.`dictionary_olympic_profiles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`dictionary_olympic_profiles` (
  `olympic_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор.',
  `profile_dict_id` INT UNSIGNED NOT NULL,
  `profile_id` INT UNSIGNED NOT NULL COMMENT 'ИД профиля олимпиады (справочник №39).',
  `level_dict_id` INT UNSIGNED NULL,
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
-- Table `PK_DB`.`profiles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`profiles` (
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'ID направления (Справочник №10).',
  `name` VARCHAR(100) NOT NULL COMMENT 'Название профиля.',
  PRIMARY KEY (`direction_id`, `name`),
  INDEX `has_idx` (`direction_id` ASC),
  CONSTRAINT `profiles_has`
    FOREIGN KEY (`direction_id`)
    REFERENCES `PK_DB`.`dictionary_10_items` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Профили обучения по направлениям.';


-- -----------------------------------------------------
-- Table `PK_DB`.`campaigns_faculties_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`campaigns_faculties_data` (
  `campaign_uid` INT UNSIGNED NOT NULL COMMENT 'Кампания.',
  `faculty_short_name` VARCHAR(5) NOT NULL COMMENT 'Факультет.',
  `hostel_places` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество мест в общежитии.',
  PRIMARY KEY (`campaign_uid`, `faculty_short_name`),
  INDEX `fk_faculties_has_campaigns_campaigns1_idx` (`campaign_uid` ASC),
  INDEX `fk_faculties_has_campaigns_faculties1_idx` (`faculty_short_name` ASC),
  CONSTRAINT `fk_faculties_has_campaigns_faculties1`
    FOREIGN KEY (`faculty_short_name`)
    REFERENCES `PK_DB`.`faculties` (`short_name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_faculties_has_campaigns_campaigns1`
    FOREIGN KEY (`campaign_uid`)
    REFERENCES `PK_DB`.`campaigns` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Данные кампаний по факультетам.';


-- -----------------------------------------------------
-- Table `PK_DB`.`campaigns_directions_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`campaigns_directions_data` (
  `campaign_uid` INT UNSIGNED NOT NULL COMMENT 'Кампания.',
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
  PRIMARY KEY (`campaign_uid`, `direction_faculty`, `direction_id`),
  INDEX `fk_directions_has_campaigns_campaigns1_idx` (`campaign_uid` ASC),
  INDEX `fk_directions_has_campaigns_directions1_idx` (`direction_id` ASC),
  CONSTRAINT `fk_directions_has_campaigns_directions1`
    FOREIGN KEY (`direction_id`)
    REFERENCES `PK_DB`.`dictionary_10_items` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_directions_has_campaigns_campaigns1`
    FOREIGN KEY (`campaign_uid`)
    REFERENCES `PK_DB`.`campaigns` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Данные кампаний по направлениям.';


-- -----------------------------------------------------
-- Table `PK_DB`.`campaigns_profiles_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`campaigns_profiles_data` (
  `profiles_direction_faculty` VARCHAR(5) NOT NULL COMMENT 'Факультет направления.',
  `profiles_direction_id` INT UNSIGNED NOT NULL COMMENT 'ID направления.',
  `profiles_name` VARCHAR(100) NOT NULL COMMENT 'Профиль.',
  `campaigns_uid` INT UNSIGNED NOT NULL COMMENT 'Кампания.',
  `places_paid_o` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество платных очных мест.',
  `places_paid_oz` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество платных вечерних мест.',
  `places_paid_z` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество платных заочных мест.',
  PRIMARY KEY (`profiles_direction_faculty`, `profiles_direction_id`, `profiles_name`, `campaigns_uid`),
  INDEX `fk_profiles_has_campaigns_campaigns1_idx` (`campaigns_uid` ASC),
  INDEX `fk_profiles_has_campaigns_profiles1_idx` (`profiles_direction_faculty` ASC, `profiles_direction_id` ASC, `profiles_name` ASC),
  CONSTRAINT `fk_profiles_has_campaigns_profiles1`
    FOREIGN KEY (`profiles_direction_id` , `profiles_name`)
    REFERENCES `PK_DB`.`profiles` (`direction_id` , `name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_profiles_has_campaigns_campaigns1`
    FOREIGN KEY (`campaigns_uid`)
    REFERENCES `PK_DB`.`campaigns` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Данные кампаний по профилям.';


-- -----------------------------------------------------
-- Table `PK_DB`.`_faculties_has_dictionary_10_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`_faculties_has_dictionary_10_items` (
  `faculties_short_name` VARCHAR(5) NOT NULL,
  `dictionary_10_items_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`faculties_short_name`, `dictionary_10_items_id`),
  INDEX `fk_faculties_has_dictionary_10_items_dictionary_10_items1_idx` (`dictionary_10_items_id` ASC),
  INDEX `fk_faculties_has_dictionary_10_items_faculties1_idx` (`faculties_short_name` ASC),
  CONSTRAINT `fk_faculties_has_dictionary_10_items_faculties1`
    FOREIGN KEY (`faculties_short_name`)
    REFERENCES `PK_DB`.`faculties` (`short_name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_faculties_has_dictionary_10_items_dictionary_10_items1`
    FOREIGN KEY (`dictionary_10_items_id`)
    REFERENCES `PK_DB`.`dictionary_10_items` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `PK_DB`.`dir_entrance_tests`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`dir_entrance_tests` (
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'ID направления (Справочник №10).',
  `subject_dict_id` INT UNSIGNED NOT NULL,
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
  `subject_dict_id` INT UNSIGNED NOT NULL,
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
  `entrant_uid` INT UNSIGNED NOT NULL COMMENT 'UID абитуриента.',
  `examination_id` INT UNSIGNED NOT NULL COMMENT 'ID экзамена.',
  `mark` SMALLINT NOT NULL COMMENT 'Оценка.',
  PRIMARY KEY (`entrant_uid`, `examination_id`),
  INDEX `entr_exam_marks_has_exam_idx` (`examination_id` ASC),
  INDEX `entr_exam_marks_has_entr_idx` (`entrant_uid` ASC),
  CONSTRAINT `entr_exam_marks_has_entr`
    FOREIGN KEY (`entrant_uid`)
    REFERENCES `PK_DB`.`entrants` (`uid`)
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
        AND campaigns_uid = id;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure get_entrants_marks
-- -----------------------------------------------------

DELIMITER $$
USE `PK_DB`$$
CREATE PROCEDURE `get_entrants_marks` (id INT UNSIGNED)
BEGIN
SELECT uid, CONCAT_WS(' ',last_name,first_name,middle_name), mark FROM
entrants_examinations_marks
JOIN
entrants ON entrant_uid=entrants.uid
WHERE examination_id=id;
END$$

DELIMITER ;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
