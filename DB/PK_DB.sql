-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

-- -----------------------------------------------------
-- Schema pk_db
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema pk_db
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `pk_db` DEFAULT CHARACTER SET utf8 ;
-- -----------------------------------------------------
-- Schema kladr
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema kladr
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `kladr` DEFAULT CHARACTER SET utf8 ;
USE `pk_db` ;

-- -----------------------------------------------------
-- Table `pk_db`.`users`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`users` (
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
-- Table `pk_db`.`dictionaries`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`dictionaries` (
  `id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор справочника.',
  `name` VARCHAR(150) NOT NULL COMMENT 'Наименование справочника.',
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB
COMMENT = 'Справочники из ФИС.';


-- -----------------------------------------------------
-- Table `pk_db`.`dictionaries_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`dictionaries_items` (
  `dictionary_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор справочника.',
  `item_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор элемента.',
  `name` VARCHAR(300) NOT NULL COMMENT 'Наименование элемента.',
  PRIMARY KEY (`dictionary_id`, `item_id`),
  INDEX `has_idx` (`dictionary_id` ASC),
  INDEX `item_id_IDX` (`item_id` ASC),
  CONSTRAINT `dictionaries_items_has`
    FOREIGN KEY (`dictionary_id`)
    REFERENCES `pk_db`.`dictionaries` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Элементы справочников ФИС.';


-- -----------------------------------------------------
-- Table `pk_db`.`campaigns`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`campaigns` (
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
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `campaigns_corresp_status`
    FOREIGN KEY (`status_id` , `status_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Приемные кампании.';


-- -----------------------------------------------------
-- Table `pk_db`.`_campaigns_has_dictionaries_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`_campaigns_has_dictionaries_items` (
  `campaigns_id` INT UNSIGNED NOT NULL,
  `dictionaries_items_dictionary_id` INT UNSIGNED NOT NULL,
  `dictionaries_items_item_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`campaigns_id`, `dictionaries_items_dictionary_id`, `dictionaries_items_item_id`),
  INDEX `fk_campaigns_has_dictionaries_items_dictionaries_items1_idx` (`dictionaries_items_dictionary_id` ASC, `dictionaries_items_item_id` ASC),
  INDEX `fk_campaigns_has_dictionaries_items_campaigns1_idx` (`campaigns_id` ASC),
  CONSTRAINT `fk_campaigns_has_dictionaries_items_campaigns1`
    FOREIGN KEY (`campaigns_id`)
    REFERENCES `pk_db`.`campaigns` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_campaigns_has_dictionaries_items_dictionaries_items1`
    FOREIGN KEY (`dictionaries_items_dictionary_id` , `dictionaries_items_item_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`dictionary_id` , `item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'campaigns:\nEducationForms.EducationFormID[1..n] - ИД формы обучения (справочник №14).\nEducationLevels.EducationLevelID[1..n] - ИД Уровня образования (справочник №2).';


-- -----------------------------------------------------
-- Table `pk_db`.`institution_achievements`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`institution_achievements` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `campaign_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор приемной кампании.',
  `name` VARCHAR(500) NOT NULL COMMENT 'Наименование индивидуального достижения.',
  `category_dict_id` INT UNSIGNED NOT NULL COMMENT '36',
  `category_id` INT UNSIGNED NOT NULL COMMENT 'ИД индивидуального достижения (справочник №36).',
  `value` SMALLINT UNSIGNED NOT NULL COMMENT 'Балл, начисляемый за индивидуальное достижение.',
  PRIMARY KEY (`id`),
  INDEX `corresp_idx` (`category_dict_id` ASC, `category_id` ASC),
  INDEX `has_idx` (`campaign_id` ASC),
  CONSTRAINT `institution_achievements_corresp`
    FOREIGN KEY (`category_id` , `category_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `institution_achievements_has`
    FOREIGN KEY (`campaign_id`)
    REFERENCES `pk_db`.`campaigns` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB
COMMENT = 'Индивидуальные достижения, учитываемые образовательной организацией.';


-- -----------------------------------------------------
-- Table `pk_db`.`target_organizations`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`target_organizations` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `name` VARCHAR(250) NOT NULL COMMENT 'Наименование целевой организации.',
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB
COMMENT = 'Целевые организации.';


-- -----------------------------------------------------
-- Table `pk_db`.`faculties`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`faculties` (
  `short_name` VARCHAR(5) NOT NULL COMMENT 'Краткое название.',
  `name` VARCHAR(75) NOT NULL COMMENT 'Название.',
  PRIMARY KEY (`short_name`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB
COMMENT = 'Факультеты.';


-- -----------------------------------------------------
-- Table `pk_db`.`dictionary_10_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`dictionary_10_items` (
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
-- Table `pk_db`.`directions`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`directions` (
  `faculty_short_name` VARCHAR(5) NOT NULL COMMENT 'Краткое название факультета.',
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'ID направления (справочник №10).',
  `short_name` VARCHAR(5) NOT NULL COMMENT 'Краткое название направления.',
  PRIMARY KEY (`faculty_short_name`, `direction_id`),
  INDEX `has_faculties_idx` (`direction_id` ASC),
  INDEX `has_dictionary_10_items_idx` (`faculty_short_name` ASC),
  CONSTRAINT `directions_has_faculties`
    FOREIGN KEY (`faculty_short_name`)
    REFERENCES `pk_db`.`faculties` (`short_name`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `directions_has_dictionary_10_items`
    FOREIGN KEY (`direction_id`)
    REFERENCES `pk_db`.`dictionary_10_items` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Направления по факультетам.';


-- -----------------------------------------------------
-- Table `pk_db`.`profiles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`profiles` (
  `faculty_short_name` VARCHAR(5) NOT NULL COMMENT 'Факультет.',
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'Направление.',
  `short_name` VARCHAR(5) NOT NULL COMMENT 'Краткое название профиля.',
  `name` VARCHAR(300) NOT NULL COMMENT 'Название профиля.',
  PRIMARY KEY (`faculty_short_name`, `direction_id`, `short_name`),
  INDEX `has_idx` (`faculty_short_name` ASC, `direction_id` ASC),
  CONSTRAINT `profiles_has`
    FOREIGN KEY (`faculty_short_name` , `direction_id`)
    REFERENCES `pk_db`.`directions` (`faculty_short_name` , `direction_id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB
COMMENT = 'Профили обучения по направлениям.';


-- -----------------------------------------------------
-- Table `pk_db`.`campaigns_faculties_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`campaigns_faculties_data` (
  `campaign_id` INT UNSIGNED NOT NULL COMMENT 'Кампания.',
  `faculty_short_name` VARCHAR(5) NOT NULL COMMENT 'Факультет.',
  `hostel_places` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество мест в общежитии.',
  PRIMARY KEY (`campaign_id`, `faculty_short_name`),
  INDEX `fk_faculties_has_campaigns_campaigns1_idx` (`campaign_id` ASC),
  INDEX `fk_faculties_has_campaigns_faculties1_idx` (`faculty_short_name` ASC),
  CONSTRAINT `fk_faculties_has_campaigns_faculties1`
    FOREIGN KEY (`faculty_short_name`)
    REFERENCES `pk_db`.`faculties` (`short_name`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_faculties_has_campaigns_campaigns1`
    FOREIGN KEY (`campaign_id`)
    REFERENCES `pk_db`.`campaigns` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB
COMMENT = 'Данные кампаний по факультетам.';


-- -----------------------------------------------------
-- Table `pk_db`.`campaigns_directions_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`campaigns_directions_data` (
  `campaign_id` INT UNSIGNED NOT NULL COMMENT 'Кампания.',
  `direction_faculty` VARCHAR(5) NOT NULL COMMENT 'Факультет направления.',
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'ID направления (Справочник №10).',
  `places_budget_o` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество бюджетных очных мест.',
  `places_budget_oz` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество бюджетных вечерних мест.',
  `places_quota_o` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество квотированных очных мест.',
  `places_quota_oz` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество квотированных вечерних мест.',
  PRIMARY KEY (`campaign_id`, `direction_faculty`, `direction_id`),
  INDEX `has_fac_dir_idx` (`direction_faculty` ASC, `direction_id` ASC),
  INDEX `has_camp_fac_d_idx` (`campaign_id` ASC, `direction_faculty` ASC),
  CONSTRAINT `campaigns_directions_data_has_fac_dir`
    FOREIGN KEY (`direction_id` , `direction_faculty`)
    REFERENCES `pk_db`.`directions` (`direction_id` , `faculty_short_name`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `campaigns_directions_data_has_camp_fac_d`
    FOREIGN KEY (`campaign_id` , `direction_faculty`)
    REFERENCES `pk_db`.`campaigns_faculties_data` (`campaign_id` , `faculty_short_name`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB
COMMENT = 'Данные кампаний по направлениям.';


-- -----------------------------------------------------
-- Table `pk_db`.`campaigns_profiles_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`campaigns_profiles_data` (
  `campaigns_id` INT UNSIGNED NOT NULL COMMENT 'Кампания.',
  `profiles_direction_faculty` VARCHAR(5) NOT NULL COMMENT 'Факультет направления.',
  `profiles_direction_id` INT UNSIGNED NOT NULL COMMENT 'ID направления.',
  `profiles_short_name` VARCHAR(5) NOT NULL COMMENT 'Профиль.',
  `places_paid_o` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество платных очных мест.',
  `places_paid_oz` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество платных вечерних мест.',
  `places_paid_z` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество платных заочных мест.',
  PRIMARY KEY (`campaigns_id`, `profiles_direction_faculty`, `profiles_direction_id`, `profiles_short_name`),
  INDEX `has_profiles_idx` (`profiles_direction_faculty` ASC, `profiles_direction_id` ASC, `profiles_short_name` ASC),
  INDEX `has_camp_dir_d_idx` (`campaigns_id` ASC, `profiles_direction_faculty` ASC, `profiles_direction_id` ASC),
  CONSTRAINT `campaigns_profiles_data_has_profiles`
    FOREIGN KEY (`profiles_direction_faculty` , `profiles_direction_id` , `profiles_short_name`)
    REFERENCES `pk_db`.`profiles` (`faculty_short_name` , `direction_id` , `short_name`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `campaigns_profiles_data_has_camp_dir_d`
    FOREIGN KEY (`campaigns_id` , `profiles_direction_faculty` , `profiles_direction_id`)
    REFERENCES `pk_db`.`campaigns_directions_data` (`campaign_id` , `direction_faculty` , `direction_id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB
COMMENT = 'Данные кампаний по профилям.';


-- -----------------------------------------------------
-- Table `pk_db`.`orders`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`orders` (
  `number` VARCHAR(50) NOT NULL COMMENT 'Номер приказа.',
  `type` ENUM('admission', 'exception', 'hostel') NOT NULL COMMENT 'Тип приказа (зачисление, исключение или выделение мест в общежитии).',
  `date` DATE NOT NULL COMMENT 'Дата регистрации приказа.',
  `protocol_number` SMALLINT UNSIGNED NULL COMMENT 'Номер протокола, если приказ зарегестрирован, иначе - NULL.',
  `protocol_date` DATE NULL COMMENT 'Дата протокола, если приказ зарегестрирован, иначе - NULL.',
  `edu_form_dict_id` INT UNSIGNED NOT NULL COMMENT '14',
  `edu_form_id` INT UNSIGNED NOT NULL COMMENT 'ИД Формы обучения (Справочник 14 \"Форма обучения\").',
  `edu_source_dict_id` INT UNSIGNED NOT NULL COMMENT '15',
  `edu_source_id` INT UNSIGNED NOT NULL COMMENT 'ИД источника финансирования (Справочник 15 \"Источник финансирования\").',
  `campaign_id` INT UNSIGNED NOT NULL COMMENT 'ID приемной кампании.',
  `faculty_short_name` VARCHAR(5) NOT NULL COMMENT 'Факультет.',
  `direction_id` INT UNSIGNED NULL COMMENT 'Направление.',
  `profile_short_name` VARCHAR(5) NULL COMMENT 'Профиль.',
  PRIMARY KEY (`number`),
  INDEX `corresp_edu_f_idx` (`edu_form_dict_id` ASC, `edu_form_id` ASC),
  INDEX `corresp_fin_s_idx` (`edu_source_dict_id` ASC, `edu_source_id` ASC),
  INDEX `orders_has_profiles_idx` (`campaign_id` ASC, `faculty_short_name` ASC, `direction_id` ASC, `profile_short_name` ASC),
  INDEX `orders_has_directions_idx` (`campaign_id` ASC, `faculty_short_name` ASC, `direction_id` ASC),
  CONSTRAINT `orders_corresp_edu_f`
    FOREIGN KEY (`edu_form_id` , `edu_form_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `orders_corresp_fin_s`
    FOREIGN KEY (`edu_source_id` , `edu_source_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `orders_has_profiles`
    FOREIGN KEY (`campaign_id` , `faculty_short_name` , `direction_id` , `profile_short_name`)
    REFERENCES `pk_db`.`campaigns_profiles_data` (`campaigns_id` , `profiles_direction_faculty` , `profiles_direction_id` , `profiles_short_name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `orders_has_directions`
    FOREIGN KEY (`campaign_id` , `faculty_short_name` , `direction_id`)
    REFERENCES `pk_db`.`campaigns_directions_data` (`campaign_id` , `direction_faculty` , `direction_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Приказы.';


-- -----------------------------------------------------
-- Table `pk_db`.`entrants`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`entrants` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `email` VARCHAR(150) NOT NULL COMMENT 'Электронный адрес.',
  `personal_password` VARCHAR(12) NOT NULL COMMENT 'Пароль к личному кабинету.',
  `custom_information` VARCHAR(4000) NULL COMMENT 'Дополнительные сведения, предоставленные абитуриентом.',
  `home_phone` VARCHAR(20) NULL COMMENT 'Домашний телефон.',
  `mobile_phone` VARCHAR(20) NULL COMMENT 'Мобильный телефон.',
  PRIMARY KEY (`id`))
ENGINE = InnoDB
COMMENT = 'Абитуриенты.';


-- -----------------------------------------------------
-- Table `pk_db`.`applications`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`applications` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `entrant_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор абитуриента.',
  `campaign_id` INT UNSIGNED NOT NULL COMMENT 'Приёмная кампания.',
  `registration_time` DATETIME NOT NULL COMMENT 'Дата и время регистрации заявления.',
  `edit_time` DATETIME NULL COMMENT 'Дата изменения.',
  `needs_hostel` TINYINT(1) NOT NULL COMMENT 'Признак необходимости общежития.',
  `status` ENUM('new', 'adm_budget', 'adm_paid', 'adm_both', 'withdrawn') NOT NULL DEFAULT 'new' COMMENT 'Статус заявления.',
  `comment` VARCHAR(4000) NULL COMMENT 'Комментарий к заявлению.',
  `language` ENUM('Английский', 'Немецкий', 'Французский') NULL COMMENT 'Изучаемый язык.',
  `registrator_login` VARCHAR(20) NOT NULL COMMENT 'Логин пользователя, занёсшего заявление.',
  `first_high_edu` TINYINT(1) NOT NULL COMMENT 'Высшее образование данного уровня получает впервые.',
  `mcado` TINYINT(1) NULL COMMENT 'МЦАДО.',
  `chernobyl` TINYINT(1) NULL COMMENT 'Чернобыльская зона.',
  `passing_examinations` TINYINT(1) NULL COMMENT 'Сдаёт экзамены.',
  `priority_right` TINYINT(1) NULL COMMENT 'Преимущественное право.',
  `special_conditions` TINYINT(1) NOT NULL COMMENT 'ВИ с созданием специальных условий.',
  `master_appl` TINYINT(1) NOT NULL COMMENT 'Заявление на уровень магистра.',
  `withdraw_date` DATE NULL COMMENT 'Дата отзыва документов.',
  `compatriot` TINYINT(1) NULL COMMENT 'Принадлежность к соотечественникам.',
  `courses` TINYINT(1) NULL COMMENT 'Курсы.',
  PRIMARY KEY (`id`),
  INDEX `has_idx` (`entrant_id` ASC),
  INDEX `applications_registrered_idx` (`registrator_login` ASC),
  INDEX `applications_camp_idx` (`campaign_id` ASC),
  CONSTRAINT `applications_has`
    FOREIGN KEY (`entrant_id`)
    REFERENCES `pk_db`.`entrants` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_registrered`
    FOREIGN KEY (`registrator_login`)
    REFERENCES `pk_db`.`users` (`login`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_camp`
    FOREIGN KEY (`campaign_id`)
    REFERENCES `pk_db`.`campaigns` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Заявления.';


-- -----------------------------------------------------
-- Table `pk_db`.`documents`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`documents` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор в ИС ОО.',
  `type` ENUM('academic_diploma', 'allow_education', 'basic_diploma', 'compatriot', 'custom', 'disability', 'edu_custom', 'high_edu_diploma', 'incopl_high_edu_diploma', 'institution', 'international_olympic', 'medical', 'middle_edu_diploma', 'olympic', 'olympic_total', 'orphan', 'parents_lost', 'pauper', 'phd_diploma', 'post_graduate_diploma', 'radiation_work', 'school_certificate', 'sport', 'state_employee', 'ukraine_olympic', 'veteran', 'ege', 'gia', 'identity', 'military_card', 'student', 'photos') NOT NULL COMMENT 'Тип документа:\nacademic_diploma\nallow_education\nbasic_diploma\ncompatriot\ncustom\ndisability\nedu_custom\nhigh_edu_diploma\nincopl_high_edu_diploma\ninstitution\ninternational_olympic\nmedical\nmiddle_edu_diploma\nolympic\nolympic_total\norphan\nparents_lost\npauper\nphd_diploma\npost_graduate_diploma\nradiation_work\nschool_certificate\nsport\nstate_employee\nukraine_olympic\nveteran\nege\ngia\nidentity\nmilitary_card\nstudent\nphotos\n',
  `series` VARCHAR(20) NULL COMMENT 'Серия документа.',
  `number` VARCHAR(100) NULL COMMENT 'Номер документа.',
  `date` DATE NULL COMMENT 'Дата выдачи документа.',
  `organization` VARCHAR(500) NULL COMMENT 'Организация, выдавшая документ.',
  `original_recieved_date` DATE NULL COMMENT 'Дата предоставления оригиналов документов.',
  PRIMARY KEY (`id`))
ENGINE = InnoDB
COMMENT = 'Документы.';


-- -----------------------------------------------------
-- Table `pk_db`.`applications_entrances`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`applications_entrances` (
  `application_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор заявления.',
  `faculty_short_name` VARCHAR(5) NOT NULL COMMENT 'Факультет.',
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'Направление.',
  `edu_form_dict_id` INT UNSIGNED NOT NULL COMMENT '14',
  `edu_form_id` INT UNSIGNED NOT NULL COMMENT 'Форма обучения (справочник №14).',
  `edu_source_dict_id` INT UNSIGNED NOT NULL COMMENT '15',
  `edu_source_id` INT UNSIGNED NOT NULL COMMENT 'Источник финансирования (справочник №15).',
  `profile_short_name` VARCHAR(5) NULL COMMENT 'Краткое имя профиля, если заявление подаётся на платную форму, иначе NULL.',
  `target_organization_id` INT UNSIGNED NULL COMMENT 'ID организации целевого приема.',
  `is_agreed_date` DATETIME NULL COMMENT 'Дата согласия на зачисление (необходимо передать при наличии согласия на зачисление).',
  `is_disagreed_date` DATETIME NULL COMMENT 'Дата отказа от зачисления (необходимо передать при включении заявления в приказ об исключении).',
  PRIMARY KEY (`application_id`, `faculty_short_name`, `direction_id`, `edu_form_dict_id`, `edu_form_id`, `edu_source_dict_id`, `edu_source_id`),
  INDEX `targets_idx` (`target_organization_id` ASC),
  INDEX `has_idx` (`application_id` ASC),
  INDEX `applies_idx` (`faculty_short_name` ASC, `direction_id` ASC),
  INDEX `corresp_edu_form_idx` (`edu_form_dict_id` ASC, `edu_form_id` ASC),
  INDEX `corresp_edu_source_idx` (`edu_source_dict_id` ASC, `edu_source_id` ASC),
  INDEX `applies_prof_idx` (`faculty_short_name` ASC, `direction_id` ASC, `profile_short_name` ASC),
  CONSTRAINT `applications_entrances_has`
    FOREIGN KEY (`application_id`)
    REFERENCES `pk_db`.`applications` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_entrances_targets`
    FOREIGN KEY (`target_organization_id`)
    REFERENCES `pk_db`.`target_organizations` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_entrances_applies`
    FOREIGN KEY (`faculty_short_name` , `direction_id`)
    REFERENCES `pk_db`.`directions` (`faculty_short_name` , `direction_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_entrances_corresp_edu_form`
    FOREIGN KEY (`edu_form_dict_id` , `edu_form_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`dictionary_id` , `item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_entrances_corresp_edu_source`
    FOREIGN KEY (`edu_source_dict_id` , `edu_source_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`dictionary_id` , `item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `applications_entrances_applies_prof`
    FOREIGN KEY (`faculty_short_name` , `direction_id` , `profile_short_name`)
    REFERENCES `pk_db`.`profiles` (`faculty_short_name` , `direction_id` , `short_name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Условия приёма заявления.';


-- -----------------------------------------------------
-- Table `pk_db`.`identity_docs_additional_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`identity_docs_additional_data` (
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
  `reg_region` VARCHAR(75) NOT NULL COMMENT 'Регион регистрации.',
  `reg_district` VARCHAR(75) NULL COMMENT 'Район регистрации.',
  `reg_town` VARCHAR(75) NULL COMMENT 'Населённый пункт регистрации.',
  `reg_street` VARCHAR(75) NULL COMMENT 'Улица регистрации.',
  `reg_house` VARCHAR(15) NULL COMMENT 'Номер дома регистрации.',
  `reg_flat` VARCHAR(6) NULL COMMENT 'Номер квартиры регистрации.',
  `reg_index` CHAR(6) NOT NULL COMMENT 'Почтовый индекс регистрации.',
  PRIMARY KEY (`document_id`),
  INDEX `corresp_gender_idx` (`gender_dict_id` ASC, `gender_id` ASC),
  INDEX `corresp_type_idx` (`type_dict_id` ASC, `type_id` ASC),
  INDEX `corresp_nation_idx` (`nationality_dict_id` ASC, `nationality_id` ASC),
  CONSTRAINT `identity_docs_additional_data_has`
    FOREIGN KEY (`document_id`)
    REFERENCES `pk_db`.`documents` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `identity_docs_additional_data_corresp_gender`
    FOREIGN KEY (`gender_id` , `gender_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `identity_docs_additional_data_corresp_type`
    FOREIGN KEY (`type_id` , `type_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `identity_docs_additional_data_corresp_nation`
    FOREIGN KEY (`nationality_id` , `nationality_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Дополнительная информация для документов, удостоверяющих личность.';


-- -----------------------------------------------------
-- Table `pk_db`.`dictionary_19_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`dictionary_19_items` (
  `olympic_id` INT UNSIGNED NOT NULL COMMENT 'ИД олимпиады.',
  `year` SMALLINT UNSIGNED NOT NULL COMMENT 'Год.',
  `olympic_number` INT UNSIGNED NULL COMMENT 'Номер олимпиады.',
  `olympic_name` VARCHAR(200) NOT NULL COMMENT 'Имя олимпиады.',
  PRIMARY KEY (`olympic_id`))
ENGINE = InnoDB
COMMENT = 'Справочник №19 \"Олимпиады\".';


-- -----------------------------------------------------
-- Table `pk_db`.`olympic_docs_additional_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`olympic_docs_additional_data` (
  `document_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор документа.',
  `diploma_type_dict_id` INT UNSIGNED NULL COMMENT '18',
  `diploma_type_id` INT UNSIGNED NULL COMMENT 'Тип диплома (справочник №18).',
  `olympic_id` INT UNSIGNED NULL COMMENT 'ИД олимпиады (справочник №19).',
  `class_number` INT UNSIGNED NULL COMMENT 'Класс обучения (7,8,9,10 или 11).',
  `olympic_name` VARCHAR(255) NULL COMMENT 'Наименование олимпиады.',
  `country_dict_id` INT UNSIGNED NULL COMMENT '7',
  `country_id` INT UNSIGNED NULL COMMENT 'Член сборной команды (справочник № 7).',
  `profile_dict_id` INT UNSIGNED NULL COMMENT '39',
  `profile_id` INT UNSIGNED NULL COMMENT 'ИД профиля олимпиады (справочник №39).',
  `olympic_subject_dict_id` INT UNSIGNED NULL COMMENT '1',
  `olympic_subject_id` INT UNSIGNED NULL COMMENT 'ИД предмета олимпиады  (должен соответствовать профилю олимпиады) (справочник № 1).',
  PRIMARY KEY (`document_id`),
  INDEX `corresp_dip_type_idx` (`diploma_type_dict_id` ASC, `diploma_type_id` ASC),
  INDEX `corresp_country_idx` (`country_dict_id` ASC, `country_id` ASC),
  INDEX `corresp_profile_idx` (`profile_dict_id` ASC, `profile_id` ASC),
  INDEX `corresp_ol_subj_idx` (`olympic_subject_dict_id` ASC, `olympic_subject_id` ASC),
  INDEX `corresp_olympic_idx` (`olympic_id` ASC),
  CONSTRAINT `olympic_docs_additional_data_has`
    FOREIGN KEY (`document_id`)
    REFERENCES `pk_db`.`documents` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `olympic_docs_additional_data_corresp_dip_type`
    FOREIGN KEY (`diploma_type_id` , `diploma_type_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `olympic_docs_additional_data_corresp_olympic`
    FOREIGN KEY (`olympic_id`)
    REFERENCES `pk_db`.`dictionary_19_items` (`olympic_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `olympic_docs_additional_data_corresp_country`
    FOREIGN KEY (`country_id` , `country_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `olympic_docs_additional_data_corresp_profile`
    FOREIGN KEY (`profile_id` , `profile_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `olympic_docs_additional_data_corresp_ol_subj`
    FOREIGN KEY (`olympic_subject_id` , `olympic_subject_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Дополнительная информация для документов по олимпиадам.';


-- -----------------------------------------------------
-- Table `pk_db`.`other_docs_additional_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`other_docs_additional_data` (
  `document_id` INT UNSIGNED NOT NULL COMMENT 'Идентфикатор документа.',
  `name` VARCHAR(1000) NULL COMMENT 'Наименование документа.',
  `dictionaries_dictionary_id` INT UNSIGNED NULL COMMENT '45, 33, 43, 42, 23, 44, 46, 47, 48',
  `dictionaries_item_id` INT UNSIGNED NULL COMMENT 'ИД элемента справочника. Для разных документов:\nveteran - VeteranCategoryID - Тип документа, подтверждающего принадлежность к участникам и ветеранам боевых действий (справочник № 45).\ninstitution: DocumentTypeID - Тип документа (справочник №33).\nsport: SportCategoryID - Тип диплома в области спорта (справочник № 43).\norphan: OrphanCategoryID - Тип документа, подтверждающего сиротство (справочник № 42).\ndisability: DisabilityTypeID - Группа инвалидности (справочник №23).\ncompatriot: CompariotCategoryID - Тип документа, подтверждающего принадлежность к соотечественникам (справочник № 44).\nparents_lost: ParentsLostCategoryID - Тип документа, подтверждающег' /* comment truncated */ /*о принадлежность родителей и опекунов к погибшим в связи с исполнением служебных обязанностей (справочник № 46).
state_employee: StateEmployeeCategoryID - Тип документа, подтверждающего принадлежность к сотрудникам государственных органов Российской Федерации (справочник № 47).
radiation_work: RadiationWorkCategoryID - Тип документа, подтверждающего участие в работах на радиационных объектах или воздействие радиации (справочник № 48).*/,
  `text_data` VARCHAR(4000) NULL COMMENT 'Текстовые данные. Для разных документов:\ncustom, sport: AdditionalInfo - Дополнительные сведения.\nedu_custom: DocumentTypeNameText - Наименование документа.',
  `year` INT UNSIGNED NULL COMMENT 'Для документа типа ege - Год выдачи свидетельства.\nПеренос diploma_docs_addituional_data.end_year - Год окончания.',
  PRIMARY KEY (`document_id`),
  INDEX `corresp_idx` (`dictionaries_dictionary_id` ASC, `dictionaries_item_id` ASC),
  CONSTRAINT `other_docs_additional_data_has`
    FOREIGN KEY (`document_id`)
    REFERENCES `pk_db`.`documents` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `other_docs_additional_data_corresp`
    FOREIGN KEY (`dictionaries_item_id` , `dictionaries_dictionary_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Дополнительная информация для остальных документов.';


-- -----------------------------------------------------
-- Table `pk_db`.`application_common_benefits`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`application_common_benefits` (
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
    REFERENCES `pk_db`.`applications` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `application_common_benefits_corresp_doc_type`
    FOREIGN KEY (`document_type_id` , `document_type_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `application_common_benefits_confirms`
    FOREIGN KEY (`reason_document_id`)
    REFERENCES `pk_db`.`documents` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `application_common_benefits_allows_education`
    FOREIGN KEY (`allow_education_document_id`)
    REFERENCES `pk_db`.`documents` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `application_common_benefits_corresp_bnf_kind`
    FOREIGN KEY (`benefit_kind_id` , `benefit_kind_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Льготы, предоставленные абитуриенту.';


-- -----------------------------------------------------
-- Table `pk_db`.`_applications_has_documents`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`_applications_has_documents` (
  `applications_id` INT UNSIGNED NOT NULL,
  `documents_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`applications_id`, `documents_id`),
  INDEX `fk_applications_has_documents_documents1_idx` (`documents_id` ASC),
  INDEX `fk_applications_has_documents_applications1_idx` (`applications_id` ASC),
  CONSTRAINT `fk_applications_has_documents_applications1`
    FOREIGN KEY (`applications_id`)
    REFERENCES `pk_db`.`applications` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_applications_has_documents_documents1`
    FOREIGN KEY (`documents_id`)
    REFERENCES `pk_db`.`documents` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB
COMMENT = 'applications:\nApplicationDocuments - Документы, приложенные к заявлению.';


-- -----------------------------------------------------
-- Table `pk_db`.`individual_achievements`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`individual_achievements` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор индивидуального достижения, учитываемого в заявлении.',
  `application_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор заявления.',
  `institution_achievement_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор достижения, указанный в приемной кампании.',
  `document_id` INT UNSIGNED NOT NULL COMMENT 'Идентификатор документа, подтверждающего индивидуальное достижение.',
  PRIMARY KEY (`id`),
  INDEX `has_idx` (`application_id` ASC),
  INDEX `gets_idx` (`institution_achievement_id` ASC),
  INDEX `confirms_idx` (`document_id` ASC),
  CONSTRAINT `individual_achievements_has`
    FOREIGN KEY (`application_id`)
    REFERENCES `pk_db`.`applications` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `individual_achievements_gets`
    FOREIGN KEY (`institution_achievement_id`)
    REFERENCES `pk_db`.`institution_achievements` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `individual_achievements_confirms`
    FOREIGN KEY (`document_id`)
    REFERENCES `pk_db`.`documents` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Индивидуальные достижения.';


-- -----------------------------------------------------
-- Table `pk_db`.`dictionary_olympic_profiles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`dictionary_olympic_profiles` (
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
    REFERENCES `pk_db`.`dictionary_19_items` (`olympic_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `dictionary_olympic_profiles_corresp_profile`
    FOREIGN KEY (`profile_id` , `profile_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `dictionary_olympic_profiles_corresp_level`
    FOREIGN KEY (`level_id` , `level_dict_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Профили олимпиады из справочника №10.';


-- -----------------------------------------------------
-- Table `pk_db`.`_dictionary_olympic_profiles_has_dictionaries_items`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`_dictionary_olympic_profiles_has_dictionaries_items` (
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
    REFERENCES `pk_db`.`dictionary_olympic_profiles` (`olympic_id` , `profile_dict_id` , `profile_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_dictionary_olympic_profiles_has_dictionaries_items_diction2`
    FOREIGN KEY (`dictionaries_items_item_id` , `dictionaries_items_dictionary_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`item_id` , `dictionary_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'dictionary_olympic_profiles:\nSubjects.SubjectID[1..n] - ИД предмета (справочник №1).';


-- -----------------------------------------------------
-- Table `pk_db`.`entrance_tests`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`entrance_tests` (
  `campaign_id` INT UNSIGNED NOT NULL COMMENT 'Кампания.',
  `direction_faculty` VARCHAR(5) NOT NULL COMMENT 'Факультет.',
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'Направление.',
  `subject_dict_id` INT UNSIGNED NOT NULL COMMENT '1',
  `subject_id` INT UNSIGNED NOT NULL COMMENT 'ID дисциплины (Справочник №1).',
  `priority` SMALLINT UNSIGNED NOT NULL COMMENT 'Приоритет.',
  PRIMARY KEY (`campaign_id`, `direction_faculty`, `direction_id`, `subject_dict_id`, `subject_id`),
  INDEX `has_idx` (`campaign_id` ASC, `direction_faculty` ASC, `direction_id` ASC),
  INDEX `corresp_idx` (`subject_dict_id` ASC, `subject_id` ASC),
  CONSTRAINT `dir_entrance_tests_has`
    FOREIGN KEY (`campaign_id` , `direction_faculty` , `direction_id`)
    REFERENCES `pk_db`.`campaigns_directions_data` (`campaign_id` , `direction_faculty` , `direction_id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `dir_entrance_tests_corresp`
    FOREIGN KEY (`subject_dict_id` , `subject_id`)
    REFERENCES `pk_db`.`dictionaries_items` (`dictionary_id` , `item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Вступительные испытания по направлениям.';


-- -----------------------------------------------------
-- Table `pk_db`.`examinations`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`examinations` (
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
    REFERENCES `pk_db`.`dictionaries_items` (`dictionary_id` , `item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Внутренние экзамены.';


-- -----------------------------------------------------
-- Table `pk_db`.`examinations_audiences`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`examinations_audiences` (
  `examination_id` INT UNSIGNED NOT NULL COMMENT 'ID экзамена.',
  `number` VARCHAR(5) NOT NULL COMMENT 'Номер аудитории.',
  `capacity` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество мест.',
  `priority` SMALLINT UNSIGNED NOT NULL COMMENT 'Приоритет аудитории.',
  PRIMARY KEY (`examination_id`, `number`),
  INDEX `has` (`examination_id` ASC),
  CONSTRAINT `examinations_audiences_has`
    FOREIGN KEY (`examination_id`)
    REFERENCES `pk_db`.`examinations` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB
COMMENT = 'Экзаменационные аудитории.';


-- -----------------------------------------------------
-- Table `pk_db`.`entrants_examinations_marks`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`entrants_examinations_marks` (
  `entrant_id` INT UNSIGNED NOT NULL COMMENT 'ID абитуриента.',
  `examination_id` INT UNSIGNED NOT NULL COMMENT 'ID экзамена.',
  `mark` SMALLINT NOT NULL DEFAULT -1 COMMENT 'Оценка.',
  PRIMARY KEY (`entrant_id`, `examination_id`),
  INDEX `entr_exam_marks_has_exam_idx` (`examination_id` ASC),
  INDEX `entr_exam_marks_has_entr_idx` (`entrant_id` ASC),
  CONSTRAINT `entr_exam_marks_has_entr`
    FOREIGN KEY (`entrant_id`)
    REFERENCES `pk_db`.`entrants` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `entr_exam_marks_has_exam`
    FOREIGN KEY (`examination_id`)
    REFERENCES `pk_db`.`examinations` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Оценки абитуриентов по внутренним экзаменам.';


-- -----------------------------------------------------
-- Table `pk_db`.`constants`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`constants` (
  `min_math_mark` SMALLINT UNSIGNED NOT NULL COMMENT 'Минимальный балл по математике.',
  `min_russian_mark` SMALLINT UNSIGNED NOT NULL COMMENT 'Минимальный балл по русскому языку.',
  `min_physics_mark` SMALLINT UNSIGNED NOT NULL COMMENT 'Минимальный балл по физике.',
  `min_social_mark` SMALLINT UNSIGNED NOT NULL COMMENT 'Минимальный балл по обществознанию.',
  `min_foreign_mark` SMALLINT UNSIGNED NOT NULL COMMENT 'Минимальный балл по иностранному языку.')
ENGINE = InnoDB
COMMENT = 'Константы.';


-- -----------------------------------------------------
-- Table `pk_db`.`campaigns_directions_target_organizations_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`campaigns_directions_target_organizations_data` (
  `campaign_id` INT UNSIGNED NOT NULL COMMENT 'Кампания.',
  `direction_faculty` VARCHAR(5) NOT NULL COMMENT 'Факультет.',
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'Направление.',
  `target_organization_id` INT UNSIGNED NOT NULL COMMENT 'ID целевой организации.',
  `places_o` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество целевых очных мест.',
  `places_oz` SMALLINT UNSIGNED NOT NULL COMMENT 'Количество целевых вечерних мест.',
  PRIMARY KEY (`campaign_id`, `direction_faculty`, `direction_id`, `target_organization_id`),
  INDEX `fk_campaigns_directions_data_has_target_organizations_targe_idx` (`target_organization_id` ASC),
  INDEX `fk_campaigns_directions_data_has_target_organizations_campa_idx` (`campaign_id` ASC, `direction_faculty` ASC, `direction_id` ASC),
  CONSTRAINT `fk_campaigns_directions_data_has_target_organizations_campaig1`
    FOREIGN KEY (`campaign_id` , `direction_faculty` , `direction_id`)
    REFERENCES `pk_db`.`campaigns_directions_data` (`campaign_id` , `direction_faculty` , `direction_id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_campaigns_directions_data_has_target_organizations_target_1`
    FOREIGN KEY (`target_organization_id`)
    REFERENCES `pk_db`.`target_organizations` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB
COMMENT = 'Данные направления кампании по целевым организациям.';


-- -----------------------------------------------------
-- Table `pk_db`.`roles_passwords`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`roles_passwords` (
  `role` ENUM('registrator', 'inspector', 'administrator') NOT NULL COMMENT 'Роль.',
  `password` VARCHAR(10) NOT NULL COMMENT 'Пароль.',
  PRIMARY KEY (`role`),
  UNIQUE INDEX `password_UNIQUE` (`password` ASC))
ENGINE = InnoDB
COMMENT = 'Пароли ролей (пользователей).';


-- -----------------------------------------------------
-- Table `pk_db`.`orders_has_applications`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`orders_has_applications` (
  `orders_number` VARCHAR(50) NOT NULL COMMENT 'Приказ.',
  `applications_id` INT UNSIGNED NOT NULL COMMENT 'Заявленеие.',
  `record_book_number` CHAR(6) NULL COMMENT 'Номер зачётной книжки (для приказов о зачислении).',
  PRIMARY KEY (`orders_number`, `applications_id`),
  INDEX `fk_orders_has_applications_applications1_idx` (`applications_id` ASC),
  INDEX `fk_orders_has_applications_orders1_idx` (`orders_number` ASC),
  CONSTRAINT `fk_orders_has_applications_orders1`
    FOREIGN KEY (`orders_number`)
    REFERENCES `pk_db`.`orders` (`number`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_orders_has_applications_applications1`
    FOREIGN KEY (`applications_id`)
    REFERENCES `pk_db`.`applications` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
COMMENT = 'Заявления в приказах.';


-- -----------------------------------------------------
-- Table `pk_db`.`masters_exams_marks`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`masters_exams_marks` (
  `campaign_id` INT UNSIGNED NOT NULL COMMENT 'Кампания.',
  `entrant_id` INT UNSIGNED NOT NULL COMMENT 'Абитуриент.',
  `faculty` VARCHAR(5) NOT NULL COMMENT 'Факультет.',
  `direction_id` INT UNSIGNED NOT NULL COMMENT 'Направление.',
  `profile_short_name` VARCHAR(5) NOT NULL COMMENT 'Программа обучения.',
  `mark` SMALLINT NOT NULL DEFAULT -1 COMMENT 'Оценка.',
  `bonus` SMALLINT UNSIGNED NOT NULL DEFAULT 0 COMMENT 'Баллы за индивидуальное достижение.',
  `date` DATE NULL COMMENT 'Дата экзамена.',
  PRIMARY KEY (`campaign_id`, `entrant_id`, `faculty`, `direction_id`, `profile_short_name`),
  INDEX `masters_exams_marks_has_camp_profile_idx` (`campaign_id` ASC, `faculty` ASC, `direction_id` ASC, `profile_short_name` ASC),
  INDEX `masters_exams_marks_has_entrant_idx` (`entrant_id` ASC),
  CONSTRAINT `masters_exams_marks_has_camp_profile`
    FOREIGN KEY (`campaign_id` , `faculty` , `direction_id` , `profile_short_name`)
    REFERENCES `pk_db`.`campaigns_profiles_data` (`campaigns_id` , `profiles_direction_faculty` , `profiles_direction_id` , `profiles_short_name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `masters_exams_marks_has_entrant`
    FOREIGN KEY (`entrant_id`)
    REFERENCES `pk_db`.`entrants` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `pk_db`.`application_ege_results`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`application_ege_results` (
  `application_id` INT UNSIGNED NOT NULL COMMENT 'Заявление.',
  `subject_dict_id` INT UNSIGNED NOT NULL COMMENT '1',
  `subject_id` INT UNSIGNED NOT NULL COMMENT 'ID дисциплины (справочник №1).',
  `series` VARCHAR(20) NULL DEFAULT NULL COMMENT 'Серия документа, подтверждающего личность, по которому сдавался ЕГЭ.',
  `number` VARCHAR(100) NOT NULL COMMENT 'Номер документа, подтверждающего личность, по которому сдавался ЕГЭ.',
  `year` SMALLINT UNSIGNED NOT NULL COMMENT 'Год сдачи.',
  `value` SMALLINT UNSIGNED NOT NULL COMMENT 'Балл.',
  `checked` TINYINT(1) NOT NULL COMMENT 'Проверен по ФИС.',
  PRIMARY KEY (`application_id`, `subject_dict_id`, `subject_id`),
  INDEX `application_ege_results_has_subjects_idx` (`subject_dict_id` ASC, `subject_id` ASC),
  INDEX `application_ege_results_has_applications_idx` (`application_id` ASC),
  CONSTRAINT `application_ege_results_has_applications`
    FOREIGN KEY (`application_id`)
    REFERENCES `local_pk_db`.`applications` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `application_ege_results_has_subjects`
    FOREIGN KEY (`subject_dict_id` , `subject_id`)
    REFERENCES `local_pk_db`.`dictionaries_items` (`dictionary_id` , `item_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8
COMMENT = 'Оценки ЕГЭ по заявлениям.';

USE `kladr` ;

-- -----------------------------------------------------
-- Table `kladr`.`subjects`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `kladr`.`subjects` (
  `name` VARCHAR(75) NOT NULL,
  `socr` VARCHAR(15) NOT NULL,
  `code` CHAR(13) NOT NULL,
  `index` CHAR(6) NULL DEFAULT NULL,
  PRIMARY KEY (`code`),
  INDEX `name_idx` (`name` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `kladr`.`streets`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `kladr`.`streets` (
  `name` VARCHAR(75) NOT NULL,
  `socr` VARCHAR(15) NOT NULL,
  `code` CHAR(17) NOT NULL,
  `index` CHAR(6) NULL DEFAULT NULL,
  PRIMARY KEY (`code`),
  INDEX `name_idx` (`name` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `kladr`.`houses`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `kladr`.`houses` (
  `name` VARCHAR(75) NOT NULL,
  `code` CHAR(19) NOT NULL,
  `index` CHAR(6) NULL DEFAULT NULL,
  PRIMARY KEY (`code`))
ENGINE = InnoDB;

USE `pk_db` ;

-- -----------------------------------------------------
-- Placeholder table for view `pk_db`.`entrants_view`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`entrants_view` (`id` INT, `last_name` INT, `first_name` INT, `middle_name` INT, `series` INT, `number` INT);

-- -----------------------------------------------------
-- Placeholder table for view `pk_db`.`applications_short_entrances_view`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`applications_short_entrances_view` (`application_id` INT, `entrances` INT);

-- -----------------------------------------------------
-- Placeholder table for view `pk_db`.`applications_documents_view`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`applications_documents_view` (`application_id` INT, `id` INT, `type` INT, `series` INT, `number` INT, `date` INT, `organization` INT, `original_recieved_date` INT);

-- -----------------------------------------------------
-- Placeholder table for view `pk_db`.`applications_orders_dates`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `pk_db`.`applications_orders_dates` (`application_id` INT, `adm_date` INT, `exc_date` INT);

-- -----------------------------------------------------
-- procedure get_campaign_edu_forms
-- -----------------------------------------------------

DELIMITER $$
USE `pk_db`$$
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
-- procedure get_application_docs
-- -----------------------------------------------------

DELIMITER $$
USE `pk_db`$$
CREATE PROCEDURE `get_application_docs` (IN id INT UNSIGNED)
BEGIN
SELECT 
    documents.id,
    type,
    series,
    number,
    date,
    organization,
    original_recieved_date
FROM
    _applications_has_documents
        JOIN
    documents ON _applications_has_documents.documents_id = documents.id
WHERE
    applications_id = id;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure get_camp_dirs_name_code
-- -----------------------------------------------------

DELIMITER $$
USE `pk_db`$$
CREATE PROCEDURE `get_camp_dirs_name_code` (IN id INT UNSIGNED)
BEGIN
SELECT 
    name, code
FROM
    campaigns_directions_data
        JOIN
    dictionary_10_items ON campaigns_directions_data.direction_id = dictionary_10_items.id
WHERE
    campaign_id = id;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure get_application_profiles
-- -----------------------------------------------------

DELIMITER $$
USE `pk_db`$$
CREATE PROCEDURE `get_application_profiles` (IN id INT UNSIGNED)
BEGIN
SELECT 
    profiles.faculty_short_name,
    profiles.direction_id,
    profiles.short_name,
    profiles.name,
    profile_actual
FROM
    applications_entrances
        JOIN
    profiles ON applications_entrances.faculty_short_name = profiles.faculty_short_name
        AND applications_entrances.direction_id = profiles.direction_id
        AND applications_entrances.profile_short_name = profiles.short_name
WHERE
    application_id = id;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure get_application_directions
-- -----------------------------------------------------

DELIMITER $$
USE `pk_db`$$
CREATE PROCEDURE `get_application_directions` (IN id INT UNSIGNED)
BEGIN
SELECT 
    faculty_short_name, dictionary_10_items.id, name, code
FROM
    applications_entrances
        JOIN
    dictionary_10_items ON applications_entrances.direction_id = dictionary_10_items.id
WHERE
    application_id = id;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure get_main_form_table
-- -----------------------------------------------------

DELIMITER $$
USE `pk_db`$$
CREATE PROCEDURE `get_main_form_table` (IN id INT UNSIGNED)
BEGIN
SELECT 
    app_data1.id,
    app_data1.last_name,
    app_data1.first_name,
    app_data1.middle_name,
    app_data1.entrances,
    app_data1.original,
    app_data1.registration_time,
    app_data1.edit_time,
    app_data1.withdraw_date,
    applications_orders_dates.adm_date,
    applications_orders_dates.exc_date,
    app_data1.registrator_login,
    app_data1.status
FROM
    (SELECT 
        app_data3.*, applications_short_entrances_view.entrances
    FROM
        applications_short_entrances_view
    JOIN (SELECT 
        app_data2.*,
            entrants_view.last_name,
            entrants_view.first_name,
            entrants_view.middle_name
    FROM
        entrants_view
    JOIN (SELECT 
        applications.id,
            applications.entrant_id,
            applications.registration_time,
            applications.edit_time,
            applications.withdraw_date,
            applications.registrator_login,
            applications.status,
            edu_docs.original
    FROM
        applications
    JOIN (SELECT 
        applications_documents_view.application_id,
            IF(applications_documents_view.original_recieved_date IS NOT NULL, TRUE, FALSE) AS original
    FROM
        applications_documents_view
    WHERE
        applications_documents_view.type = 'academic_diploma'
            OR applications_documents_view.type = 'school_certificate'
            OR applications_documents_view.type = 'middle_edu_diploma'
            OR applications_documents_view.type = 'high_edu_diploma') AS edu_docs
            ON applications.id = edu_docs.application_id WHERE applications.campaign_id = id) AS app_data2
            ON app_data2.entrant_id = entrants_view.id) AS app_data3
            ON applications_short_entrances_view.application_id = app_data3.id) AS app_data1
        LEFT OUTER JOIN
    applications_orders_dates
    ON app_data1.id = applications_orders_dates.application_id;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- View `pk_db`.`entrants_view`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `pk_db`.`entrants_view`;
USE `pk_db`;
CREATE  OR REPLACE VIEW `entrants_view` AS
    SELECT DISTINCT
        entrants.id,
        appls_idents.last_name,
        appls_idents.first_name,
        appls_idents.middle_name,
        appls_idents.series,
        appls_idents.number
    FROM
        entrants
            JOIN
        (SELECT 
            applications.entrant_id,
                a_d_idents.series,
                a_d_idents.number,
                a_d_idents.last_name,
                a_d_idents.first_name,
                a_d_idents.middle_name
        FROM
            applications
        JOIN (SELECT 
            _applications_has_documents.applications_id,
                docs_idents.series,
                docs_idents.number,
                docs_idents.last_name,
                docs_idents.first_name,
                docs_idents.middle_name
        FROM
            _applications_has_documents
        JOIN (SELECT 
            documents.id,
                documents.series,
                documents.number,
                identity_docs_additional_data.last_name,
                identity_docs_additional_data.first_name,
                identity_docs_additional_data.middle_name
        FROM
            documents
        JOIN identity_docs_additional_data ON documents.id = identity_docs_additional_data.document_id) AS docs_idents ON _applications_has_documents.documents_id = docs_idents.id) AS a_d_idents ON applications.id = a_d_idents.applications_id) AS appls_idents ON entrants.id = appls_idents.entrant_id;

-- -----------------------------------------------------
-- View `pk_db`.`applications_short_entrances_view`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `pk_db`.`applications_short_entrances_view`;
USE `pk_db`;
CREATE  OR REPLACE VIEW `applications_short_entrances_view` AS
    SELECT 
        applications_entrances.application_id,
        GROUP_CONCAT(IFNULL(CONCAT(applications_entrances.profile_short_name, '*'), directions.short_name) SEPARATOR ', ') AS entrances
    FROM
        applications_entrances
            JOIN
        directions ON applications_entrances.faculty_short_name = directions.faculty_short_name
            AND applications_entrances.direction_id = directions.direction_id
    GROUP BY applications_entrances.application_id;

-- -----------------------------------------------------
-- View `pk_db`.`applications_documents_view`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `pk_db`.`applications_documents_view`;
USE `pk_db`;
CREATE  OR REPLACE VIEW `applications_documents_view` AS
    SELECT 
        _applications_has_documents.applications_id AS application_id,
        documents.id,
        type,
        series,
        number,
        date,
        organization,
        original_recieved_date
    FROM
        _applications_has_documents
            JOIN
        documents ON _applications_has_documents.documents_id = documents.id;

-- -----------------------------------------------------
-- View `pk_db`.`applications_orders_dates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `pk_db`.`applications_orders_dates`;
USE `pk_db`;
CREATE  OR REPLACE VIEW `applications_orders_dates` AS
    SELECT 
        adm_ord.applications_id AS application_id,
        adm_ord.date AS adm_date,
        exc_ord.date AS exc_date
    FROM
        (SELECT 
            orders_has_applications.applications_id,
                MAX(orders.date) AS date
        FROM
            orders_has_applications
        JOIN orders ON orders_has_applications.orders_number = orders.number
        WHERE
            orders.`type` = 'admission'
        GROUP BY orders_has_applications.applications_id) AS adm_ord
            LEFT OUTER JOIN
        (SELECT 
            orders_has_applications.applications_id,
                MAX(orders.date) AS date
        FROM
            orders_has_applications
        JOIN orders ON orders_has_applications.orders_number = orders.number
        WHERE
            orders.`type` = 'exception'
        GROUP BY orders_has_applications.applications_id) AS exc_ord ON adm_ord.applications_id = exc_ord.applications_id;
CREATE USER 'initial' IDENTIFIED BY '1234';

GRANT SELECT ON TABLE `pk_db`.`users` TO 'initial';
GRANT SELECT ON TABLE `pk_db`.`roles_passwords` TO 'initial';
GRANT INSERT, SELECT ON TABLE `pk_db`.`constants` TO 'initial';
CREATE USER 'registrator' IDENTIFIED BY 'reg1234';

GRANT SELECT ON TABLE `pk_db`.`users` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`campaigns` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`dictionaries_items` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`dictionaries` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`institution_achievements` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`target_organizations` TO 'registrator';
GRANT INSERT, SELECT, UPDATE ON TABLE `pk_db`.`applications` TO 'registrator';
GRANT INSERT, SELECT, UPDATE ON TABLE `pk_db`.`entrants` TO 'registrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`documents` TO 'registrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`applications_entrances` TO 'registrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`identity_docs_additional_data` TO 'registrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`olympic_docs_additional_data` TO 'registrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`other_docs_additional_data` TO 'registrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`application_common_benefits` TO 'registrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`_applications_has_documents` TO 'registrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`individual_achievements` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`dictionary_10_items` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`dictionary_19_items` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`dictionary_olympic_profiles` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`_dictionary_olympic_profiles_has_dictionaries_items` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`faculties` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`profiles` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`campaigns_faculties_data` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`campaigns_directions_data` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`campaigns_profiles_data` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`directions` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`examinations` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`entrants_examinations_marks` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`constants` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`campaigns_directions_target_organizations_data` TO 'registrator';
GRANT SELECT ON TABLE `kladr`.`subjects` TO 'registrator';
GRANT SELECT ON TABLE `kladr`.`streets` TO 'registrator';
GRANT SELECT ON TABLE `kladr`.`houses` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`entrants_view` TO 'registrator';
GRANT EXECUTE ON procedure `pk_db`.`get_application_directions` TO 'registrator';
GRANT EXECUTE ON procedure `pk_db`.`get_application_docs` TO 'registrator';
GRANT EXECUTE ON procedure `pk_db`.`get_application_profiles` TO 'registrator';
GRANT EXECUTE ON procedure `pk_db`.`get_camp_dirs_name_code` TO 'registrator';
GRANT EXECUTE ON procedure `pk_db`.`get_campaign_edu_forms` TO 'registrator';
GRANT SELECT ON TABLE `pk_db`.`_campaigns_has_dictionaries_items` TO 'registrator';
GRANT EXECUTE ON procedure `pk_db`.`get_main_form_table` TO 'registrator';
GRANT INSERT, SELECT, UPDATE ON TABLE `pk_db`.`application_ege_results` TO 'registrator';
CREATE USER 'inspector' IDENTIFIED BY 'ins1234';

GRANT SELECT ON TABLE `pk_db`.`users` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`campaigns` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`dictionaries_items` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`dictionaries` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`institution_achievements` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`target_organizations` TO 'inspector';
GRANT INSERT, SELECT, UPDATE ON TABLE `pk_db`.`applications` TO 'inspector';
GRANT INSERT, SELECT, UPDATE ON TABLE `pk_db`.`entrants` TO 'inspector';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`documents` TO 'inspector';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`applications_entrances` TO 'inspector';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`identity_docs_additional_data` TO 'inspector';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`olympic_docs_additional_data` TO 'inspector';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`other_docs_additional_data` TO 'inspector';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`application_common_benefits` TO 'inspector';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`_applications_has_documents` TO 'inspector';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`individual_achievements` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`dictionary_10_items` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`dictionary_19_items` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`dictionary_olympic_profiles` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`_dictionary_olympic_profiles_has_dictionaries_items` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`faculties` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`profiles` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`campaigns_faculties_data` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`campaigns_directions_data` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`campaigns_profiles_data` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`directions` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`examinations` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`entrants_examinations_marks` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`constants` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`campaigns_directions_target_organizations_data` TO 'inspector';
GRANT SELECT ON TABLE `kladr`.`subjects` TO 'inspector';
GRANT SELECT ON TABLE `kladr`.`streets` TO 'inspector';
GRANT SELECT ON TABLE `kladr`.`houses` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`entrants_view` TO 'inspector';
GRANT EXECUTE ON procedure `pk_db`.`get_application_directions` TO 'inspector';
GRANT EXECUTE ON procedure `pk_db`.`get_application_docs` TO 'inspector';
GRANT EXECUTE ON procedure `pk_db`.`get_application_profiles` TO 'inspector';
GRANT EXECUTE ON procedure `pk_db`.`get_camp_dirs_name_code` TO 'inspector';
GRANT EXECUTE ON procedure `pk_db`.`get_campaign_edu_forms` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`_campaigns_has_dictionaries_items` TO 'inspector';
GRANT EXECUTE ON procedure `pk_db`.`get_main_form_table` TO 'inspector';
GRANT INSERT, SELECT, UPDATE ON TABLE `pk_db`.`application_ege_results` TO 'inspector';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`orders` TO 'inspector';
GRANT SELECT ON TABLE `pk_db`.`entrance_tests` TO 'inspector';
GRANT SELECT, DELETE, UPDATE, INSERT ON TABLE `pk_db`.`examinations` TO 'inspector';
GRANT DELETE, INSERT, UPDATE, SELECT ON TABLE `pk_db`.`examinations_audiences` TO 'inspector';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`entrants_examinations_marks` TO 'inspector';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`orders_has_applications` TO 'inspector';
GRANT INSERT, SELECT, UPDATE ON TABLE `pk_db`.`masters_exams_marks` TO 'inspector';
CREATE USER 'administrator' IDENTIFIED BY 'adm1234';

GRANT SELECT ON TABLE `pk_db`.`users` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`campaigns` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`dictionaries_items` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`dictionaries` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`institution_achievements` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`target_organizations` TO 'administrator';
GRANT INSERT, SELECT, UPDATE ON TABLE `pk_db`.`applications` TO 'administrator';
GRANT INSERT, SELECT, UPDATE ON TABLE `pk_db`.`entrants` TO 'administrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`documents` TO 'administrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`applications_entrances` TO 'administrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`identity_docs_additional_data` TO 'administrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`olympic_docs_additional_data` TO 'administrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`other_docs_additional_data` TO 'administrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`application_common_benefits` TO 'administrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`_applications_has_documents` TO 'administrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`individual_achievements` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`dictionary_10_items` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`dictionary_19_items` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`dictionary_olympic_profiles` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`_dictionary_olympic_profiles_has_dictionaries_items` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`faculties` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`profiles` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`campaigns_faculties_data` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`campaigns_directions_data` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`campaigns_profiles_data` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`directions` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`examinations` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`entrants_examinations_marks` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`constants` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`campaigns_directions_target_organizations_data` TO 'administrator';
GRANT SELECT ON TABLE `kladr`.`subjects` TO 'administrator';
GRANT SELECT ON TABLE `kladr`.`streets` TO 'administrator';
GRANT SELECT ON TABLE `kladr`.`houses` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`entrants_view` TO 'administrator';
GRANT EXECUTE ON procedure `pk_db`.`get_application_directions` TO 'administrator';
GRANT EXECUTE ON procedure `pk_db`.`get_application_docs` TO 'administrator';
GRANT EXECUTE ON procedure `pk_db`.`get_application_profiles` TO 'administrator';
GRANT EXECUTE ON procedure `pk_db`.`get_camp_dirs_name_code` TO 'administrator';
GRANT EXECUTE ON procedure `pk_db`.`get_campaign_edu_forms` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`_campaigns_has_dictionaries_items` TO 'administrator';
GRANT EXECUTE ON procedure `pk_db`.`get_main_form_table` TO 'administrator';
GRANT INSERT, SELECT, UPDATE ON TABLE `pk_db`.`application_ege_results` TO 'administrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`orders` TO 'administrator';
GRANT SELECT ON TABLE `pk_db`.`entrance_tests` TO 'administrator';
GRANT SELECT, DELETE, UPDATE, INSERT ON TABLE `pk_db`.`examinations` TO 'administrator';
GRANT DELETE, INSERT, UPDATE, SELECT ON TABLE `pk_db`.`examinations_audiences` TO 'administrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`entrants_examinations_marks` TO 'administrator';
GRANT DELETE, INSERT, SELECT, UPDATE ON TABLE `pk_db`.`orders_has_applications` TO 'administrator';
GRANT INSERT, SELECT, UPDATE ON TABLE `pk_db`.`masters_exams_marks` TO 'administrator';
GRANT DELETE, INSERT, UPDATE ON TABLE `pk_db`.`users` TO 'administrator';
GRANT DELETE, INSERT, UPDATE ON TABLE `pk_db`.`campaigns` TO 'administrator';
GRANT INSERT, UPDATE ON TABLE `pk_db`.`dictionaries_items` TO 'administrator';
GRANT INSERT, UPDATE ON TABLE `pk_db`.`dictionaries` TO 'administrator';
GRANT DELETE, INSERT, UPDATE ON TABLE `pk_db`.`_campaigns_has_dictionaries_items` TO 'administrator';
GRANT DELETE, INSERT, UPDATE ON TABLE `pk_db`.`institution_achievements` TO 'administrator';
GRANT DELETE, INSERT, UPDATE ON TABLE `pk_db`.`target_organizations` TO 'administrator';
GRANT INSERT, UPDATE ON TABLE `pk_db`.`dictionary_10_items` TO 'administrator';
GRANT INSERT, UPDATE ON TABLE `pk_db`.`dictionary_19_items` TO 'administrator';
GRANT INSERT, UPDATE ON TABLE `pk_db`.`dictionary_olympic_profiles` TO 'administrator';
GRANT INSERT, UPDATE ON TABLE `pk_db`.`_dictionary_olympic_profiles_has_dictionaries_items` TO 'administrator';
GRANT DELETE, INSERT, UPDATE ON TABLE `pk_db`.`faculties` TO 'administrator';
GRANT DELETE, INSERT, UPDATE ON TABLE `pk_db`.`profiles` TO 'administrator';
GRANT DELETE, INSERT, UPDATE ON TABLE `pk_db`.`campaigns_faculties_data` TO 'administrator';
GRANT DELETE, INSERT, UPDATE ON TABLE `pk_db`.`campaigns_directions_data` TO 'administrator';
GRANT DELETE, INSERT, UPDATE ON TABLE `pk_db`.`campaigns_profiles_data` TO 'administrator';
GRANT DELETE, INSERT, UPDATE ON TABLE `pk_db`.`directions` TO 'administrator';
GRANT DELETE, UPDATE, INSERT ON TABLE `pk_db`.`entrance_tests` TO 'administrator';
GRANT UPDATE, INSERT ON TABLE `pk_db`.`constants` TO 'administrator';
GRANT DELETE, INSERT, UPDATE ON TABLE `pk_db`.`campaigns_directions_target_organizations_data` TO 'administrator';
GRANT DELETE, INSERT ON TABLE `kladr`.`subjects` TO 'administrator';
GRANT DELETE, INSERT ON TABLE `kladr`.`streets` TO 'administrator';
GRANT DELETE, INSERT ON TABLE `kladr`.`houses` TO 'administrator';

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;

-- -----------------------------------------------------
-- Data for table `pk_db`.`constants`
-- -----------------------------------------------------
START TRANSACTION;
USE `pk_db`;
INSERT INTO `pk_db`.`constants` (`min_math_mark`, `min_russian_mark`, `min_physics_mark`, `min_social_mark`, `min_foreign_mark`) VALUES (0, 0, 0, 0, 0);

COMMIT;

