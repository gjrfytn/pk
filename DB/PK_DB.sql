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
  `name` VARCHAR(75) NOT NULL COMMENT 'Наименование элемента.',
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
  `uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор в ИС ОО.',
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
-- Table `PK_DB`.`admission_volumes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`admission_volumes` (
  `uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор в ИС ОО.',
  `campaign_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор приёмной кампании (UID).',
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
  `admission_volume_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор объема приема по направлению подготовки.',
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
  `institution_achievement_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор в ИС ОО.',
  `name` VARCHAR(500) NOT NULL COMMENT 'Наименование индивидуального достижения.',
  `id_category` INT UNSIGNED NOT NULL COMMENT 'ИД индивидуального достижения (справочник №36).',
  `max_value` SMALLINT UNSIGNED NOT NULL COMMENT 'Максимальный балл, начисляемый за индивидуальное достижение.',
  `campaign_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор приемной кампании.',
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
  `uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор в ИС ОО.',
  `name` VARCHAR(250) NOT NULL COMMENT 'Наименование целевой организации.',
  PRIMARY KEY (`uid`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB
COMMENT = 'Целевые организации.';


-- -----------------------------------------------------
-- Table `PK_DB`.`orders`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`orders` (
  `uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор в ИС ОО.',
  `campaign_uid` INT UNSIGNED NOT NULL COMMENT 'UID приемной кампании.',
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


-- -----------------------------------------------------
-- Table `PK_DB`.`competitive_groups`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`competitive_groups` (
  `uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор в ИС ОО.',
  `campaign_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор приемной кампании (UID).',
  `name` VARCHAR(250) NOT NULL COMMENT 'Наименование конкурса.',
  `education_level_id` INT UNSIGNED NOT NULL COMMENT 'ИД уровня образования (справочник №2).',
  `education_source_id` INT UNSIGNED NOT NULL COMMENT 'ИД источника финансирования (справочник №15).',
  `education_form_id` INT UNSIGNED NOT NULL COMMENT 'ИД формы обучения (справочник №14).',
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'ИД направления подготовки (справочник №10).',
  `is_for_krym` TINYINT(1) NOT NULL COMMENT 'Конкурс для граждан Крыма.',
  `is_additional` TINYINT(1) NOT NULL COMMENT 'Конкурс для дополнительного набора.',
  `places_type` ENUM('budget_o', 'budget_oz', 'budget_z', 'paid_o', 'paid_oz', 'paid_z', 'quota_o', 'quota_oz', 'quota_z', 'target_o', 'target_oz', 'target_z') NOT NULL COMMENT 'Тип мест в конкурсной группе.',
  `places_number` INT UNSIGNED NOT NULL COMMENT 'Количество мест в конкурсной группе.',
  PRIMARY KEY (`uid`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC),
  INDEX `has_idx` (`campaign_uid` ASC),
  INDEX `corresponds_edu_lvl_idx` (`education_level_id` ASC),
  INDEX `corresponds_edu_src_idx` (`education_source_id` ASC),
  INDEX `corresponds_edu_form_idx` (`education_form_id` ASC),
  INDEX `corresponds_direction_idx` (`direction_id` ASC),
  CONSTRAINT `has`
    FOREIGN KEY (`campaign_uid`)
    REFERENCES `PK_DB`.`campaigns` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_edu_lvl`
    FOREIGN KEY (`education_level_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_edu_src`
    FOREIGN KEY (`education_source_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_edu_form`
    FOREIGN KEY (`education_form_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_direction`
    FOREIGN KEY (`direction_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Конкурсные группы (конкурсы).';


-- -----------------------------------------------------
-- Table `PK_DB`.`edu_programs`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`edu_programs` (
  `uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор в ИС ОО.',
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
  `edu_form_id` INT UNSIGNED NOT NULL COMMENT 'Форма обучения.',
  `places_number` INT UNSIGNED NOT NULL COMMENT 'Количество мест.',
  PRIMARY KEY (`target_organization_uid`, `competitive_group_uid`),
  INDEX `has_idx` (`competitive_group_uid` ASC),
  INDEX `corresponds_edu_form_idx` (`edu_form_id` ASC),
  CONSTRAINT `has`
    FOREIGN KEY (`competitive_group_uid`)
    REFERENCES `PK_DB`.`competitive_groups` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_t_org`
    FOREIGN KEY (`target_organization_uid`)
    REFERENCES `PK_DB`.`target_organizations` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_edu_form`
    FOREIGN KEY (`edu_form_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Сведения о целевом наборе от организаций.';


-- -----------------------------------------------------
-- Table `PK_DB`.`entrance_tests`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`entrance_tests` (
  `uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор в ИС ОО.',
  `entrance_test_type_id` INT UNSIGNED NOT NULL COMMENT 'Тип вступительного испытания (справочник №11).',
  `min_score` SMALLINT UNSIGNED NOT NULL COMMENT 'Минимальное количество баллов.',
  `entrance_test_priority` INT UNSIGNED NOT NULL COMMENT 'Приоритет вступительного испытания.',
  `entrance_test_subject_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор дисциплины вступительного испытания (справочник №1).',
  `is_for_spo_and_vo` INT UNSIGNED NULL COMMENT 'UID заменяемого испытания.\nЕсли испытание для поступающих на основании профильного СПО/ВО, иначе - NULL.',
  `competitive_group_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор конкурсной группы.',
  PRIMARY KEY (`uid`),
  INDEX `corresponds_test_type_idx` (`entrance_test_type_id` ASC),
  INDEX `corresponds_test_subject_idx` (`entrance_test_subject_id` ASC),
  INDEX `replaces_test_idx` (`is_for_spo_and_vo` ASC),
  INDEX `has_idx` (`competitive_group_uid` ASC),
  CONSTRAINT `corresponds_test_type`
    FOREIGN KEY (`entrance_test_type_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_test_subject`
    FOREIGN KEY (`entrance_test_subject_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `replaces_test`
    FOREIGN KEY (`is_for_spo_and_vo`)
    REFERENCES `PK_DB`.`entrance_tests` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `has`
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
  `uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор в ИС ОО.',
  `benefit_kind_id` INT UNSIGNED NOT NULL COMMENT 'Вид льготы (справочник №30).',
  `is_for_all_olympics` TINYINT(1) NOT NULL COMMENT 'Флаг действия льготы для всех олимпиад.',
  `is_creative` TINYINT(1) NULL COMMENT 'Творческие олимпиады.\nNULL, если льгота относится к вступительному испытанию.',
  `is_athletic` TINYINT(1) NULL COMMENT 'Олимпиады в области спорта.\nNULL, если льгота относится к вступительному испытанию.',
  `level_for_all_olympics` INT UNSIGNED NOT NULL COMMENT 'Уровни для всех олимпиад (справочник №3).',
  `class_for_all_olympics` INT UNSIGNED NOT NULL COMMENT 'Классы для всех олимпиад (справочник №40).',
  `competitive_group_uid` INT UNSIGNED NULL COMMENT 'Идентификатор конкурсной группы.\nNULL, если льгота относится к вступительному испытанию.',
  `min_ege_mark` INT UNSIGNED NULL COMMENT 'Минимальный балл ЕГЭ, при котором можно использовать льготу.\nNULL, если льгота относится к конкурсной группе.',
  `entrance_test_id` INT UNSIGNED NULL COMMENT 'Идентификатор вступительного испытания.\nNULL, если льгота относится к конкурсной группе.',
  PRIMARY KEY (`uid`),
  INDEX `corresponds_bnf_kind_idx` (`benefit_kind_id` ASC),
  INDEX `corresponds_lvl_for_all_idx` (`level_for_all_olympics` ASC),
  INDEX `corresponds_cls_for_all_idx` (`class_for_all_olympics` ASC),
  INDEX `has_idx` (`competitive_group_uid` ASC),
  INDEX `has_entr_test_idx` (`entrance_test_id` ASC),
  CONSTRAINT `corresponds_bnf_kind`
    FOREIGN KEY (`benefit_kind_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_lvl_for_all`
    FOREIGN KEY (`level_for_all_olympics`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_cls_for_all`
    FOREIGN KEY (`class_for_all_olympics`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `has_comp_gr`
    FOREIGN KEY (`competitive_group_uid`)
    REFERENCES `PK_DB`.`competitive_groups` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `has_entr_test`
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
-- Table `PK_DB`.`benefit_olympics_levels`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`benefit_olympics_levels` (
  `benefit_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор льготы.',
  `olympic_id` INT UNSIGNED NOT NULL COMMENT 'ИД олимпиады (справочник №19).',
  `level_id` INT UNSIGNED NOT NULL COMMENT 'Уровни олимпиады (справочник №3).',
  `class_id` INT UNSIGNED NOT NULL COMMENT 'Классы олимпиады (справочник №40).',
  PRIMARY KEY (`olympic_id`, `benefit_uid`),
  INDEX `corresponds_lvl_id_idx` (`level_id` ASC),
  INDEX `corresponds_class_id_idx` (`class_id` ASC),
  INDEX `has_idx` (`benefit_uid` ASC),
  CONSTRAINT `corresponds_olymp`
    FOREIGN KEY (`olympic_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_lvl`
    FOREIGN KEY (`level_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds_class`
    FOREIGN KEY (`class_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `has`
    FOREIGN KEY (`benefit_uid`)
    REFERENCES `PK_DB`.`entrance_benefits` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Параметры олимпиад, определяющие льготу.';


-- -----------------------------------------------------
-- Table `PK_DB`.`_benefit_olympics_levels_has_dictionaries_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`_benefit_olympics_levels_has_dictionaries_items` (
  `benefit_olympics_levels_olympic_id` INT UNSIGNED NOT NULL,
  `benefit_olympics_levels_common_benefit_uid` INT UNSIGNED NOT NULL,
  `dictionaries_items_item_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`benefit_olympics_levels_olympic_id`, `benefit_olympics_levels_common_benefit_uid`, `dictionaries_items_item_id`),
  INDEX `fk_olympics_levels_has_dictionaries_items_dictionaries_item_idx` (`dictionaries_items_item_id` ASC),
  INDEX `fk_olympics_levels_has_dictionaries_items_olympics_levels1_idx` (`benefit_olympics_levels_olympic_id` ASC, `benefit_olympics_levels_common_benefit_uid` ASC),
  CONSTRAINT `fk_benefit_olympics_levels_has_dictionaries_items_olympics_levels1`
    FOREIGN KEY (`benefit_olympics_levels_olympic_id` , `benefit_olympics_levels_common_benefit_uid`)
    REFERENCES `PK_DB`.`benefit_olympics_levels` (`olympic_id` , `benefit_uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_olympics_levels_has_dictionaries_items_dictionaries_items1`
    FOREIGN KEY (`dictionaries_items_item_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'olympics_levels:\nProfiles.ProfileID[1..n] - ИД профиля олимпиады (справочник №39).';


-- -----------------------------------------------------
-- Table `PK_DB`.`benefit_min_ege_marks`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `PK_DB`.`benefit_min_ege_marks` (
  `benefit_uid` INT UNSIGNED NOT NULL COMMENT 'Идентификатор льготы.',
  `subject_id` INT UNSIGNED NOT NULL COMMENT 'ИД дисциплины (справочник №1).',
  `min_mark` INT UNSIGNED NOT NULL COMMENT 'Минимальная оценка по предмету.',
  PRIMARY KEY (`benefit_uid`, `subject_id`),
  INDEX `corresponds_idx` (`subject_id` ASC),
  CONSTRAINT `has`
    FOREIGN KEY (`benefit_uid`)
    REFERENCES `PK_DB`.`entrance_benefits` (`uid`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `corresponds`
    FOREIGN KEY (`subject_id`)
    REFERENCES `PK_DB`.`dictionaries_items` (`item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Минимальная оценка по предметам для льготы.';


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
