// agent => address
// avatar => agent_address, address

CREATE TABLE IF NOT EXISTS `users` (
  `user_id`             VARCHAR,
  `avatar_address`      VARCHAR,
  `agent_address`       VARCHAR, // unique
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC)
);

CREATE TABLE IF NOT EXISTS `users` (
  `user_id`             VARCHAR,
  `avatar_address`      VARCHAR,
  `agent_address`       VARCHAR, // unique
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC)
);

CREATE TABLE IF NOT EXISTS `hack_and_slash` (
  //`user_id`              VARCHAR,
  // avatar_address
  // agent_address
  `stage_id`             VARCHAR,
  // cleared bool?
  INDEX `idx_fk_user_id` (`user_id` ASC),
  CONSTRAINT `fk_hack_and_slash_users`
    FOREIGN KEY (`user_id`)
    REFERENCES `data_provider`.`users` (`user_id`)
);
  // succeed 된 케이스만 넣는다